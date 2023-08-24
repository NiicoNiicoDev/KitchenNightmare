using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class PathFinding
{
    #region MinimumSpanningTree
    public static Branch[] MinimumSpanningTree(Branch[] branches)
    {
        //Starts the MST at the first branch
        Branch currentBranch = branches[0];
        currentBranch.newPathways = Branch.ReOrderPathways(currentBranch.newPathways);

        //Starts the pathway at the (ordered by distance) first path
        Route currentRoute = currentBranch.newPathways[0];

        List<Branch> mst = new List<Branch>();

        List<Branch> completedBranches = new List<Branch>();

        List<Branch> duplicateBranches = new List<Branch>();

        int tracker = 0;

        while (true)
        {
            //Stops the MST trying to backtrack on the last branch
            if (mst.Count == branches.Length - 1)
            {
                break;
            }

            //Ensures a new branch isn't already in the MST
            if (!completedBranches.Contains(currentRoute.nextBranch))
            {
                //mst.Add(new Branch(currentBranch, currentRoute));
                if (mst.Contains(currentBranch))
                {
                    duplicateBranches.Add(new Branch(currentBranch, currentRoute));
                    duplicateBranches[duplicateBranches.Count - 1].shortestPath = currentRoute;
                }
                else
                {
                    mst.Add(currentBranch);
                    currentBranch.shortestPath = currentRoute;
                }

                completedBranches.Add(currentBranch);

                //Every route has access to the next branch
                currentBranch = currentRoute.nextBranch;

                currentBranch.newPathways = Branch.ReOrderPathways(currentBranch.newPathways);
                //Debug to clarify if a branch doesn't have any connections
                if (currentBranch.newPathways.Count == 0)
                {
                    throw new System.Exception("Current Branch Broken On Seed = " + Object.FindObjectOfType<SCR_ProceduralGeneration>().seed);
                }
                currentRoute = currentBranch.newPathways[0];

                tracker++;
                continue;
            }
            else
            {
                //If the current route leads to taken branch - remove it
                currentBranch.newPathways.RemoveAt(0);

                //If a backtrack needs to take place
                if (currentBranch.newPathways.Count == 0)
                {
                    //Uncommon case but makes MST look nicer
                    if (mst.Count == branches.Length - 2)
                    {
                        //Immediately links the final two branches
                        Branch finalBranch = Branch.FindMissingBranch(completedBranches, branches);
                        finalBranch.newPathways = Branch.ReOrderPathways(finalBranch.newPathways);
                        if (finalBranch.newPathways.Count != 0) 
                        {
                            //mst.Add(new Branch(finalBranch, finalBranch.newPathways[0]));
                            mst.Add(finalBranch);
                            finalBranch.shortestPath = finalBranch.newPathways[0];
                            completedBranches.Add(finalBranch);
                            break;
                        }
                    }

                    //Backtracking through the MST
                    while (true)
                    {

                        mst.Add(currentBranch);
                        currentBranch.doNotPathFind = true;

                        int index = completedBranches.Count - 1;
                        index -= 1;

                        //Uses the index to backtrack in the branch list
                        currentBranch = completedBranches[index];
                        //Gets a new route to check
                        if (currentBranch.newPathways.Count > 1)
                        {
                            currentBranch.newPathways.RemoveAt(0);
                            currentRoute = currentBranch.newPathways[0];
                            break;
                        }
                        else if (mst.Count == branches.Length - 1)
                        {
                            break;
                        }
                    }
                    tracker++;
                }
                //If no backtrack is needed - follow the next path along
                else
                {
                    currentRoute = currentBranch.newPathways[0];
                }
            }

            if (tracker >= 1000)
            {
                throw new System.ArgumentException("Stuck inside loop");
            }
        }

        foreach(Branch dupe in duplicateBranches)
        {
            mst.Add(dupe);
        }

        /*foreach (Branch allBranches in branches)
        {
            bool exists = false;
            foreach (Branch node in mst)
            {
                if (allBranches.root == node.root)
                {
                    exists = true;
                }
            }
            if (!exists)
            {
                mst.Add(new Branch(allBranches));
                mst[mst.Count - 1].doNotPathFind = true;
            }
        }*/

        Branch lastBranch = Branch.FindMissingBranch(mst, branches);
        mst.Add(lastBranch);
        lastBranch.doNotPathFind = true;

        return mst.ToArray();
    }


    public static Branch[] GetRandomLoops(Branch[] branches, int randomLoopNumber)
    {
        List<Branch> newBranchList = new List<Branch>();
        List<Branch> currentLoops = new List<Branch>();
        for (int i = 0; i < branches.Length; i++)
        {
            newBranchList.Add(branches[i]);
        }

        //Starting at 1, every 10 rooms another link is added
        //int randomLoopNumber = (branches.Length / 10) + 1;

        for (int i = 0; i < randomLoopNumber; i++)
        {
            //Finds a random branch and adds it to current loops
            Branch randomBranch = newBranchList[Random.Range(0, newBranchList.Count)];
            while (currentLoops.Contains(randomBranch) || randomBranch.newPathways.Count <= 2)
            {
                randomBranch = newBranchList[Random.Range(0, newBranchList.Count)];
            }

            //Ensures the pathways are in the correct order
            randomBranch.originalPathways = Branch.ReOrderPathways(randomBranch.originalPathways);

            Route newEdge = new Route();

            //Loops through original pathways and breaks when it finds an unused pathway
            for (int j = 0; j < randomBranch.originalPathways.Count; j++)
            {
                newEdge = new Route(randomBranch.originalPathways[j]);
                //randomBranch.shortestPath = randomBranch.originalPathways[j];
                bool doesExist = false;

                foreach (Branch existingBranch in newBranchList)
                {
                    if (existingBranch.shortestPath == newEdge)
                    {
                        doesExist = true;
                        break;
                    }
                }
                //If path is not used break out of loop
                if (!doesExist)
                {
                    break;
                }
            }

            //Add branch to the current loops

            Branch newBranch = new Branch(randomBranch.root, newEdge);
            currentLoops.Add(newBranch);
            //currentLoops.Add(randomBranch);
        }
        foreach(Branch loop in currentLoops)
        {
            newBranchList.Add(loop);
        }

        return newBranchList.ToArray();
    }
    #endregion

    public static void InstantiateHallway(GameObject startObj, GameObject endObj, GameObject straightHallwayPrefab, GameObject turnHallwayPrefab, int gridSnapValue, int pathNum)
    {
        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();
        
        //Used for hallway spawning
        Vector3 startWorldSpace = startObj.transform.position + (startObj.transform.forward * (gridSnapValue / 2));
        Vector3 endWorldSpace = endObj.transform.position + (endObj.transform.forward * (gridSnapValue / 2));

        //Used for simpler pathfinding comparison
        Vector2 startPoint = new Vector2(startWorldSpace.x, startWorldSpace.z);
        Vector2 endPoint = new Vector2(endWorldSpace.x, endWorldSpace.z);

        Node start = new Node(startPoint, endPoint, startPoint, startPoint, startObj, endObj);
        start.parent = start;
        openList.Add(start);

        int tracker = 0;
        //Accounts for all nodes not checked
        while (openList.Count != 0)
        {
            tracker++;
            if (tracker >= 1000)
            {
                throw new System.ArgumentException("Stuck in infinite loop!");
            }

            //Ensures the node with the lowest cost is always checked first
            Node currentNode = openList[0];
            foreach (Node node in openList)
            {
                if (node.sum < currentNode.sum)
                {
                    currentNode = node;
                }
            }
            //Removes node from the list as it is about to be checked
            openList.Remove(currentNode);
            Node[] paths = new Node[4];

            //Creates 4 pathways for all possible cardinal directions
            paths[0] = new Node(currentNode.position, endPoint, currentNode.position + (Vector2.up * gridSnapValue), startPoint, startObj, endObj);
            paths[1] = new Node(currentNode.position, endPoint, currentNode.position + (Vector2.down * gridSnapValue), startPoint, startObj, endObj);
            paths[2] = new Node(currentNode.position, endPoint, currentNode.position + (Vector2.right * gridSnapValue), startPoint, startObj, endObj);
            paths[3] = new Node(currentNode.position, endPoint, currentNode.position + (Vector2.left * gridSnapValue), startPoint, startObj, endObj);

            foreach(Node node in paths)
            {
                node.parent = currentNode;
                if (node.position == endPoint)
                {
                    GameObject container = GameObject.FindGameObjectWithTag("Dungeon_Container");
                    GameObject parent = new GameObject("Pathway " + pathNum);
                    parent.transform.parent = container.transform;
                    
                    //Ensures the final node can be spawned
                    closedList.Add(node);
                    closedList.Add(currentNode);
                    node.isBlocked = false;

                    //Used for backtracing through nodes for spawning
                    Node closedListNode = node;
                    Node previousClosedListNode = node;
                    while (true)
                    {
                        SpawnHallwayPrefab(closedListNode, previousClosedListNode, straightHallwayPrefab, turnHallwayPrefab, parent.transform, gridSnapValue);

                        //If the current node has reached the end of the backtrace
                        if (closedListNode == closedListNode.parent)
                        {
                            break;
                        }
                        //Otherwise set the current node to its own parent and keep moving
                        previousClosedListNode = closedListNode;
                        closedListNode = closedListNode.parent;
                    }
                    //Destroys the two doors (Where they will be unlocked)

                    foreach (SCR_OpenDoor doorScript in startObj.GetComponentsInChildren<SCR_OpenDoor>())
                    {
                        doorScript.UnlockDoor();
                    }

                    foreach (SCR_OpenDoor doorScript in endObj.GetComponentsInChildren<SCR_OpenDoor>())
                    {
                        doorScript.UnlockDoor();
                    }
                    return;
                }

                bool shouldSkipNode = false;
                //Ensures any of the 4 new nodes are not duplicates of the open list nodes
                foreach (Node openListNode in openList)
                {
                    if (openListNode.position == node.position && openListNode.sum < node.sum)
                    {
                        shouldSkipNode = true;
                        break;
                    }
                }
                if (shouldSkipNode) { continue; }

                shouldSkipNode = false;
                //The same again is considered for nodes in the closed (checked) list
                foreach (Node closedListNode in closedList)
                {
                    if (closedListNode.position == node.position && closedListNode.sum < node.sum)
                    {
                        shouldSkipNode = true;
                        break;
                    }
                }
                if (shouldSkipNode) { continue; }

                //Positions for raycast collision check
                Vector3 worldPos = new Vector3(node.position.x, 10.0f, node.position.y);
                Vector3 nodePos = new Vector3(node.position.x, -10.0f, node.position.y);

                bool isBlocked = Physics.Linecast(worldPos, nodePos);
                //Blocked indicates whether nodes should be spawned once the end point is found
                if (isBlocked)
                {
                    closedList.Add(node);
                    node.isBlocked = true;
                }
                else
                {
                    openList.Add(node);
                    node.isBlocked = false;
                }
            }

            closedList.Add(currentNode);
        }

        //There a literally no more paths to take
        if (openList.Count == 0)
        {
            throw new System.ArgumentException("Ran out of pathways");
        }
    }

    private static void SpawnHallwayPrefab(Node closedListNode, Node previousClosedListNode, GameObject straightHallwayPrefab, GameObject turnHallwayPrefab, Transform parent, int gridSnapValue)
    {
        //Used to orienting the hallway prefab
        Vector3 currentPos = new Vector3(closedListNode.position.x, 0.0f, closedListNode.position.y);
        Vector3 parentPos = new Vector3(closedListNode.parent.position.x, 0.0f, closedListNode.parent.position.y);
        Vector3 prevPos = new Vector3(previousClosedListNode.position.x, 0, previousClosedListNode.position.y);

        //Direction vectors
        Vector3 newDir = (parentPos - currentPos).normalized;
        Vector3 prevDir = (currentPos - prevPos).normalized;

        Quaternion direction = Quaternion.identity;

        //The final node would not have a direction
        if (newDir != Vector3.zero)
        {
            direction = Quaternion.LookRotation(newDir, Vector3.up);
        }
        //Indicator == 1 if its the start piece, 2 if its the end piece, and 0 any other time
        int hallwayIndicator = (previousClosedListNode == closedListNode) ? 1 : (closedListNode == closedListNode.parent) ? 2 : 0;

        //If the hallway is start or end
        if (hallwayIndicator > 0)
        {
            //The end piece should face the previous hallway || otherwise the direction will remain the same
            if (hallwayIndicator == 2)
            {
                direction = Quaternion.LookRotation(prevDir, Vector3.up);
            }
            Object.Instantiate(straightHallwayPrefab, currentPos, direction, parent.transform);
            return;
        }

        //This distance detects if a corner piece is required
        if (Vector2.Distance(previousClosedListNode.position, closedListNode.parent.position) < 3)
        {
            //Looks back on itself first
            direction = Quaternion.LookRotation(prevDir, Vector3.up);
            GameObject hallway = Object.Instantiate(turnHallwayPrefab, currentPos, direction, parent.transform);
            
            //Hallway should be rotated 1 of 2 ways == if its transform.right does not point towards the next piece: rotate 90 degrees
            if (hallway.transform.right != newDir.normalized)
            {
                hallway.transform.rotation *= Quaternion.Euler(0, 90, 0);
            }
            return;
        }

        //Spawns a straight piece using standard direction
        Object.Instantiate(straightHallwayPrefab, currentPos, direction, parent.transform);
    }

    private class Node
    {
        public float movementCost;
        public float heuristic;
        public float sum;

        public Node parent;
        public Vector2 position;
        public bool isBlocked;

        public Node(Vector2 previousNode, Vector2 endPoint, Vector2 currentPos, Vector2 startPoint, GameObject startObj, GameObject endObj)
        {
            movementCost = Vector2.Distance(previousNode, currentPos);
            heuristic = Mathf.Abs(currentPos.x - endPoint.x) + Mathf.Abs(currentPos.y - endPoint.y);

            //heuristic = Mathf.Sqrt(Mathf.Pow((currentPos.x - endPoint.x), 2) + Mathf.Pow((currentPos.y - endPoint.y), 2));

            sum = movementCost + heuristic;
            //sum = movementCost + heuristic - Mathf.Sqrt(distanceToObjective);
            position = currentPos;
            isBlocked = false;
        }
    }

}
