using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using TMPro;

public class SCR_ProceduralGeneration : MonoBehaviour
{
    [Header("Procedural Generation Settings\n")]
    [Tooltip("The predefined space that all rooms will be spawned in")]
    [SerializeField] private Vector2 spawningBounds;
    public Procedural_Gen_Settings scriptableObject;

    [Tooltip("List of rooms to be spawned (excluding start and boss)")]
    [SerializeField] private GameObject[] roomPrefabs;
    [Tooltip("Number of rooms to be randomly spawned")]
    [SerializeField] private int numberOfRooms = 1;

    [SerializeField] private GameObject startRoom;
    [SerializeField] private GameObject bossRoom;

    [SerializeField] private GameObject straightHallwayPrefab;
    [SerializeField] private GameObject turnHallwayPrefab;

    [SerializeField] private int randomLoopNumber = 2;

    /*[Tooltip("The radius that will be used to check if rooms are overlapping")]
    [SerializeField] private float overlapRadius;*/  //deprecated - replaced with overlap radius on SCR_RoomSettings on each room prefab
    [Tooltip("The multiple on which rooms and hallways are snapped to")]
    [SerializeField] private int gridSnapValue;
    public TextAsset testedSeedAsset;
    [SerializeField] private TextMeshProUGUI seedTextDisplay;

    [Header("Triangulation Settings\n")]
    [SerializeField] private bool showTriangulation = false;
    [SerializeField] private bool showCommonEdge = false;
    [SerializeField] private bool showDelaunayEdge = false;
    [SerializeField] private bool showMST = false;
    [SerializeField] private bool showOverlapRadius = false;
    public bool killOnFail = false;

    [HideInInspector] public int seed;
    //RandomSeed and UseTested seeds are overwritten in editor script - to change values at runtime they must be done so in Start()
    [HideInInspector] public bool randomSeed = false;
    [HideInInspector] public bool useTestedSeeds = false;

    [HideInInspector] public int numberOfNewSeeds;
    private int[] testedSeedArray = new int[0];
    [SerializeField] private TextAsset seedTextAsset;
    [HideInInspector] public bool sucessfulGeneration { get; private set; } = true;
    [HideInInspector] public bool isTesting = false;



    private GameObject[] dungeon;

    private Vertex[] vertices;

    private List<Edge> edges = new List<Edge>();

    private List<Triangle> triangles = new List<Triangle>();

    private List<Edge> commonEdges = new List<Edge>();

    private List<Edge> delaunayEdges = new List<Edge>();

    private Branch[] minimumSpanningTree;


    private void Start()
    {
        randomSeed = scriptableObject.useRandomSeed;
        useTestedSeeds = scriptableObject.useTestedSeeds;

        //useTestedSeeds = true;
        ResetGeneration();
        GameManager.gameManager.DistributeSpawners();
        SCR_AudioManager.instance.PlayMusic("Level 1 music");
        Destroy(this);
    }

    public void GenerateDungeon()
    {
        //Can be removed on final build (Used for editor)
        DestroyImmediate(GameObject.FindGameObjectWithTag("Dungeon_Container"));
        sucessfulGeneration = true;
        isTesting = false;

        if (randomSeed)
        {
            seed = UnityEngine.Random.Range(1, 100000);
        }
        //Pulls seeds from tested txt file
        else if(useTestedSeeds)
        {
            if (testedSeedArray.Length == 0)
            {
                ReadSeedsFromFile();
            }

            UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
            int index = UnityEngine.Random.Range(0, testedSeedArray.Length - 1);
            seed = testedSeedArray[index];

            int safetyCounter = 0;
            while (seed == GameManager.gameManager.previousSeed) //The seed is the same as last time
            {
                index = UnityEngine.Random.Range(0, testedSeedArray.Length - 1);
                seed = testedSeedArray[index];

                safetyCounter++;
                if (safetyCounter >= 1000)
                {
                    seed = testedSeedArray[0];
                    break;
                }
            }

            GameManager.gameManager.previousSeed = seed;
        }
        else
        {
            seed = scriptableObject.seed;
        }
        Debug.Log("Starting Seed = " + seed);

        //Creates a list of gameObjects that represents the spawned dungeon
        dungeon = Dungeon.Instantiate(spawningBounds, roomPrefabs, startRoom, bossRoom, gridSnapValue, seed, numberOfRooms);

        GenerateVertices();
    }

    private void GenerateVertices()
    {
        //Resizes vertices list
        vertices = new Vertex[dungeon.Length];

        //Loops through all vertices and adds the room positions to the list
        for (int i = 0; i < vertices.Length; i++)
        {
            float x = dungeon[i].transform.position.x;
            float y = dungeon[i].transform.position.z;

            vertices[i] = new Vertex(x, y);
        }
    }

    public void TriangulateDungeon()
    {
        //Can be removed on final build (Used for editor)
        if (edges.Count > 0)
        {
            edges.Clear();
        }
        if (triangles.Count > 0)
        {
            triangles.Clear();
        }

        //Sort list according to x position
        vertices = vertices.OrderBy(vertex => vertex.x).ToArray();

        //Triangulates vertices and stores them in a triangle list
        triangles = Triangulate.Incremental(vertices);

        //Loop through all triangles and add all their edges to an edge list
        for (int i = 0; i < triangles.Count; i++)
        {
            //Ensure all triangles are stored in counterclockwise order
            if (Triangle.IsTriangleClockwise(triangles[i]))
            {
                triangles[i].ReOrder();
            }

            edges.Add(triangles[i].e1);
            edges.Add(triangles[i].e2);
            edges.Add(triangles[i].e3);
        }

        commonEdges = Edge.FindCommonEdges(edges);

        DelaunayTriangulation();
    }

    private void DelaunayTriangulation()
    {
        //Returns a list of edges that have been correctly rearranged/flipped
        delaunayEdges = Triangulate.Delaunay(commonEdges);

        //Create a list of outeredges that currently contains all original edges
        List<Edge> outerEdges = Edge.CloneEdges(edges);
        List<Edge> temp = new List<Edge>();

        //Loop through all common edges and locate duplicates in outer edges
        for (int i = 0; i < commonEdges.Count; i++)
        {
            for (int j = 0; j < outerEdges.Count; j++)
            {
                //Add all instances of duplicates to the temp list
                if (commonEdges[i] == outerEdges[j])
                {
                    temp.Add(outerEdges[j]);
                }
            }
        }

        //Remove all duplicates from the outer edges list - leaves only outer edges as they only have 1 instance of themself
        for (int i = 0; i < temp.Count; i++)
        {
            outerEdges.Remove(temp[i]);
        }
        //Adds outer edges to the delaunay list to complete the triangulation
        for (int i = 0; i < outerEdges.Count; i++)
        {
            delaunayEdges.Add(outerEdges[i]);
        }
    }

    public void StartTestingSeeds(int seed)
    {
        //Used to stop the generation after 1 failure
        isTesting = true;

        //Can be removed on final build (Used for editor)
        DestroyImmediate(GameObject.FindGameObjectWithTag("Dungeon_Container"));

        sucessfulGeneration = true;
        this.seed = seed;

        dungeon = Dungeon.Instantiate(spawningBounds, roomPrefabs, startRoom, bossRoom, gridSnapValue, this.seed, numberOfRooms);

        GenerateVertices();
        TriangulateDungeon();
        MinimumSpanningTree();
    }

    public void ReadSeedsFromFile()
    {
        string seedText = seedTextAsset.text;
        string[] lines = seedText.Replace("\r", "").Split('\n');
        testedSeedArray = new int[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i] == "")
            {
                continue;
            }
            testedSeedArray[i] = int.Parse(lines[i]);
        }
    }

    public void MinimumSpanningTree()
    {
        //Creates new data structure that contains each room and its connections
        Branch[] allBranches = Branch.CreateBranchList(vertices, delaunayEdges);

        try
        {
            minimumSpanningTree = PathFinding.MinimumSpanningTree(allBranches);
        }
        catch (System.Exception)
        {
            DungeonFailure();
            return;
        }

        minimumSpanningTree = PathFinding.GetRandomLoops(minimumSpanningTree, randomLoopNumber);


        List<Vertex> connectedDungeon = new List<Vertex>();
        List<Branch> failedConnections = new List<Branch>();
        //Loops through the MST and spawns the hallways
        for (int i = 0; i < minimumSpanningTree.Length; i++)
        {
            if (minimumSpanningTree[i].doNotPathFind) { continue; }

            //Locates which doors to start at for each room
            GameObject startDoor = Dungeon.FindClosestDoorway(minimumSpanningTree[i].root, minimumSpanningTree[i].shortestPath, dungeon);
            GameObject endDoor = Dungeon.FindClosestDoorway(minimumSpanningTree[i].shortestPath.v2, minimumSpanningTree[i].shortestPath, dungeon);

            //Attempts to pathfind but throws an error if it cannot
            try
            {
                PathFinding.InstantiateHallway(startDoor, endDoor, straightHallwayPrefab, turnHallwayPrefab, gridSnapValue, i);

                connectedDungeon.Add(minimumSpanningTree[i].root);
                //connectedDungeon.Add(minimumSpanningTree[i].shortestPath.nextBranch.root);

                //minimumSpanningTree[i].finalDestination = minimumSpanningTree[i].shortestPath.nextBranch;

                minimumSpanningTree[i].connectedRooms++;
                minimumSpanningTree[i].shortestPath.nextBranch.connectedRooms++;
            }
            //Catches any failed connections for attempted fixing later
            catch (System.ArgumentException)
            {
                if (!failedConnections.Contains(minimumSpanningTree[i]))
                {
                    failedConnections.Add(minimumSpanningTree[i]);
                }
            }
        }
        if (failedConnections.Count > 0)
        {
            //CorrectBrokenPathways(connectedDungeon, failedConnections);
            AttemptFixBrokenPathways(connectedDungeon, failedConnections);
        }


        if (!IsValidDungeon2())
        {
            //throw new System.Exception("Dungeon is not fully connected");
            DungeonFailure();
            return;
        }

    }

    private void AddRandomPathways(Branch[] pathways)
    {
        for (int i = 0; i < randomLoopNumber; i++)
        {
            int index = UnityEngine.Random.Range(0, pathways.Length);

            if (!CanFindPossiblePath(pathways[index], i))
            {
                continue;
            }
        }
    }

    private bool IsValidDungeon()
    {
        List<Branch> noDuplicates = new List<Branch>();
        noDuplicates.Add(minimumSpanningTree[0]);
        for (int i = 1; i < minimumSpanningTree.Length; i++)
        {
            bool doesExist = false;
            for (int j = 0; j < noDuplicates.Count; j++)
            {
                if (minimumSpanningTree[i].root == noDuplicates[j].root)
                {
                    doesExist = true;
                    minimumSpanningTree[i].connectedRooms += noDuplicates[j].connectedRooms;
                }
            }
            if (!doesExist)
            {
                noDuplicates.Add(minimumSpanningTree[i]);
            }
        }

        int roomsWithOneConnection = 0;
        bool singleRoom = false;
        for (int i = 0; i < noDuplicates.Count; i++)
        {
            Debug.Log(noDuplicates[i].connectedRooms);

            if (noDuplicates[i].connectedRooms == 1)
            {
                roomsWithOneConnection++;
            }
            else if (noDuplicates[i].connectedRooms == 0)
            {
                singleRoom = true;
            }
        }

        if (singleRoom || roomsWithOneConnection > 2)
        {
            return false;
        }

        return true;
    }

    private bool IsValidDungeon2()
    {
        List<Branch> noDuplicates = new List<Branch>();
        noDuplicates.Add(minimumSpanningTree[0]);
        for (int i = 1; i < minimumSpanningTree.Length; i++)
        {
            bool doesExist = false;
            for (int j = 0; j < noDuplicates.Count; j++)
            {
                if (minimumSpanningTree[i].root == noDuplicates[j].root)
                {
                    doesExist = true;
                    minimumSpanningTree[i].connectedRooms += noDuplicates[j].connectedRooms;
                }
            }
            if (!doesExist)
            {
                noDuplicates.Add(minimumSpanningTree[i]);
            }
        }

        foreach(Branch branch in noDuplicates)
        {
            if (branch.connectedRooms == 0) { return false; }

            if (branch.root == noDuplicates[0].root || branch.root == noDuplicates[noDuplicates.Count - 1].root)
            {
                continue;
            }

            if (branch.connectedRooms <= 1)
            {
                //Debug.Log(branch.root.WorldPosition + " = Broken Room");
                /*try
                {
                    if (branch.shortestPath.nextBranch.connectedRooms < 2)
                    {
                        return false;
                    }
                }
                catch (System.NullReferenceException)
                {
                    return false;
                }*/
                return false;
            }
        }

        return true;
    }

    public void ResetGeneration()
    {
        ResetFields();
        GenerateDungeon();
        TriangulateDungeon();
        MinimumSpanningTree();
    }

    public void ResetFields()
    {
        dungeon = new GameObject[0];
        vertices = new Vertex[0];
        edges.Clear();
        triangles.Clear();
        commonEdges.Clear();
        delaunayEdges.Clear();
        minimumSpanningTree = new Branch[0];
        sucessfulGeneration = true;
    }

    private void CorrectBrokenPathways(List<Vertex> connectedDungeon, List<Branch> failedConnections)
    {
        int redundantPathways = 0;
        for (int i = 0; i < failedConnections.Count; i++)
        {

            bool startRoomConnected = false;
            bool endRoomConnected = false;

            //For the failed connection check if the rooms it connects are already part of the dungeon
            foreach (Vertex room in connectedDungeon)
            {
                if (room == failedConnections[i].root)
                {
                    startRoomConnected = true;
                }
                else if (room == failedConnections[i].shortestPath.nextBranch.root)
                {
                    endRoomConnected = true;
                }
            }

            //If both rooms are already connected - the pathway is redundant
            if (startRoomConnected && endRoomConnected)
            {
                //Pathway can be safely ignored
                Debug.Log("Pathway " + i + " is broken but redundant");
                redundantPathways++;
                if (redundantPathways >= 2)
                {
                    sucessfulGeneration = false;
                    //DungeonFailure();
                    return;
                }
                continue;
            }
            else
            {
                //Figures out which rooms still needs a connection and attempts to do it
                if (!startRoomConnected)
                {
                    if (!CanFindPossiblePath(failedConnections[i], i))
                    {
                        DungeonFailure();
                        return;
                    }
                }
                if (!endRoomConnected)
                {
                    if (!CanFindPossiblePath(failedConnections[i].shortestPath.nextBranch, i))
                    {
                        DungeonFailure();
                        return;
                    }
                }
            }
        }
    }

    private void AttemptFixBrokenPathways(List<Vertex> connectedDungeon, List<Branch> failedConnections)
    {
        for (int i = 0; i < failedConnections.Count; i++)
        {
            if (!CanFindPossiblePath(failedConnections[i], i))
            {
                //DungeonFailure();
                return;
            }

            connectedDungeon.Add(failedConnections[i].root);
        }
    }

    private void DungeonFailure()
    {
        if (killOnFail) { throw new System.ArgumentException("Stopped Dungeon Regenerating"); } //Debugging value that stops a seed from continuing to generate

        sucessfulGeneration = false;
        if (isTesting) { return; }

        //Regenerate Dungeon
        Debug.Log("Dungeon Regenerated On Seed = " + seed);
        seed = UnityEngine.Random.Range(1, 100000);
        ResetGeneration();
    }

    private bool CanFindPossiblePath(Branch brokenRoom, int i)
    {
        brokenRoom.originalPathways = Branch.ReOrderPathways(brokenRoom.originalPathways);

        //Loops through all old pathways and just tries to build them (ignoring whether they're already in use)
        for (int j = 0; j < brokenRoom.originalPathways.Count; j++)
        {
            try
            {
                GameObject startDoor = Dungeon.FindClosestDoorway(brokenRoom.root, brokenRoom.originalPathways[j], dungeon);
                GameObject endDoor = Dungeon.FindClosestDoorway(brokenRoom.originalPathways[j].nextBranch.root, brokenRoom.originalPathways[j], dungeon);
                PathFinding.InstantiateHallway(startDoor, endDoor, straightHallwayPrefab, turnHallwayPrefab, gridSnapValue, i);
                //Debug.Log("Replaced pathway " + i);

                //brokenRoom.finalDestination = brokenRoom.originalPathways[j].nextBranch;

                brokenRoom.connectedRooms++;
                brokenRoom.originalPathways[j].nextBranch.connectedRooms++;
                return true;
            }
            catch (System.ArgumentException)
            {
                //Attempts any remaining pathways despite the errors
                if (j == brokenRoom.originalPathways.Count - 1)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public GameObject[] GetDungeonRooms
    {
        get { return dungeon; }
    }

    #region Gizmos
    private void OnDrawGizmos()
    {
        try
        {
            Gizmos.color = Color.yellow;

            for (int i = 0; i < vertices.Length; i++)
            {
                Gizmos.DrawIcon(vertices[i].WorldPosition, "Gizmo.tiff", true);
            }

            if (showTriangulation == true)
            {
                for (int i = 0; i < edges.Count; i++)
                {
                    Gizmos.DrawLine(edges[i].v1.WorldPosition, edges[i].v2.WorldPosition);
                }
            }

            if (showCommonEdge == true)
            {
                Gizmos.color = Color.red;
                for (int i = 0; i < commonEdges.Count; i++)
                {
                    Gizmos.DrawLine(commonEdges[i].v1.WorldPosition, commonEdges[i].v2.WorldPosition);
                }
            }

            if (showDelaunayEdge == true)
            {
                Gizmos.color = Color.green;
                for (int i = 0; i < delaunayEdges.Count; i++)
                {
                    Gizmos.DrawLine(delaunayEdges[i].v1.WorldPosition, delaunayEdges[i].v2.WorldPosition);
                }
            }

            if (showMST == true)
            {
                Gizmos.color = Color.blue;
                for (int i = 0; i < minimumSpanningTree.Length; i++)
                {
                    Gizmos.DrawLine(minimumSpanningTree[i].shortestPath.v1.WorldPosition, minimumSpanningTree[i].shortestPath.v2.WorldPosition);
                }
            }

            if (showOverlapRadius == true)
            {
                Gizmos.color = Color.cyan;
                for (int i = 0; i < dungeon.Length; i++)
                {
                    Gizmos.DrawWireSphere(dungeon[i].transform.position, dungeon[i].GetComponent<SCR_RoomSettings>().overlapRadius);
                }
            }
        }
        catch (NullReferenceException)
        {
            return;
        }

    }
    #endregion
}
