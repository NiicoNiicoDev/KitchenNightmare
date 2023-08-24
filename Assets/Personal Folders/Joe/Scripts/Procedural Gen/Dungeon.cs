using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public static class Dungeon
{
    ///<summary>
    ///Returns a list of all rooms spawned
    ///</summary>
    public static GameObject[] Instantiate(Vector2 constraints, GameObject[] rooms, GameObject start, GameObject boss, int gridSnapValue, int seed, int numberOfRooms)
    {
        GameObject[] dungeon = new GameObject[numberOfRooms];

        Random.InitState(seed);

        //An object is created to contain all instantiated rooms
        GameObject parent = new GameObject("Dungeon");
        parent.tag = "Dungeon_Container";

        //Position of the start/tutorial room
        Vector3 startPos = RandomPosition(constraints, gridSnapValue);

        //Position of the boss room alongside an offset from the start room
        Vector3 bossPos = RandomPosition(constraints, gridSnapValue);
        bossPos = Offset(bossPos, startPos, constraints, gridSnapValue);

        dungeon[0] = Object.Instantiate(start, startPos, Quaternion.identity, parent.transform);
        dungeon[1] = Object.Instantiate(boss, bossPos, Quaternion.identity, parent.transform);

        //Loops through all rooms and spawns them - then adds them to a list
        for (int i = 2; i < numberOfRooms; i++)
        {
            int randomIndex = Random.Range(0, rooms.Length);

            Vector3 randPos = RandomPosition(constraints, gridSnapValue);

            float overlapRadius = rooms[randomIndex].GetComponent<SCR_RoomSettings>().overlapRadius;

            int failSafe = 0;

            while (Physics.OverlapSphere(randPos, overlapRadius).Length > 0)
            {
                randPos = RandomPosition(constraints, gridSnapValue);

                failSafe++;
                //Fail safe prevents the code from getting stuck in an infinite loop
                if (failSafe >= 1000)
                {
                    Object.DestroyImmediate(parent);
                    seed = Random.Range(1, 100000);
                    return Instantiate(constraints, rooms, start, boss, gridSnapValue, seed, numberOfRooms);
                }
            }

            dungeon[i] = Object.Instantiate(rooms[randomIndex], randPos, GetRotation(), parent.transform);
            dungeon[i].name += " " + i;
        }

        return dungeon;
    }

    private static Quaternion GetRotation()
    {
        int r = Random.Range(0, 4);
        switch(r)
        {
            case (0):
                return Quaternion.identity;
            case (1):
                return Quaternion.Euler(0, 90, 0);
            case (2):
                return Quaternion.Euler(0, 180, 0);
            case (3):
                return Quaternion.Euler(0, 270, 0);
        }
        return Quaternion.identity;
    }

    ///<summary>
    ///Takes two vectors and returns the 1st a set distance away from the 2nd
    ///</summary>
    private static Vector3 Offset(Vector3 pos1, Vector3 pos2, Vector2 constraints, int gridSnapValue)
    {
        //Creates a random distance from 1 an arbitrary distance defined by the constraints - it is cast to int to avoid decimal values
        int minimumOffset = (int)(constraints.x + constraints.y) / 5;

        //Whilst the boss room position is smaller than the offset...randomize position
        while (Vector3.Distance(pos1, pos2) < minimumOffset)
        {
            pos1 = RandomPosition(constraints, gridSnapValue);
        }

        return pos1;
    }

    ///<summary>
    ///Generates a random Vector3 position between two points
    ///</summary>
    private static Vector3 RandomPosition(Vector2 constraints, int gridSnapValue)
    {
        //Returns a random vector3 between two points and casts the values to ints to ensure there are no decimal values
        int x = (Random.Range(1, (int)constraints.x) / gridSnapValue) * gridSnapValue;
        int z = (Random.Range(1, (int)constraints.y) / gridSnapValue) * gridSnapValue;

        return new Vector3(x, 0, z);
    }

    /// <summary>
    /// Returns the door to be destroyed
    /// </summary>
    /// <param name="roomPos"></param>
    /// <param name="direction"></param>
    /// <param name="dungeon"></param>
    /// <returns></returns>
    public static GameObject FindClosestDoorway(Vertex roomPos, Edge direction, GameObject[] dungeon)
    {
        GameObject currentRoom = dungeon[0];
        //Locates the room game object
        foreach (GameObject room in dungeon)
        {
            if (room.transform.position == roomPos.WorldPosition)
            {
                currentRoom = room;
                break;
            }
        }

        //Halfway point between room 1 and room 2
        Vector3 halfwayPoint = (direction.v1.WorldPosition + direction.v2.WorldPosition) / 2;

        GameObject doorwayContainer = currentRoom;

        //Locates a parent game object that contains all the doors
        foreach (Transform child in currentRoom.transform)
        {
            if (child.CompareTag("Doorway_Container"))
            {
                doorwayContainer = child.gameObject;
                break;
            }
        }
        //Tries to find an available door to unlock
        GameObject closestDoor = doorwayContainer;
        try
        {
            //Closest door to pathway is currently the first in the list
            closestDoor = doorwayContainer.transform.GetChild(0).gameObject;
        }
        catch(UnityException)
        {
            Debug.Log("There are no doorways in container " + doorwayContainer.transform.parent.name);
        }

        //Start comparing the distance of all subsequent doors to the halfway point
        for (int i = 1; i < doorwayContainer.transform.childCount; i++)
        {
            if (Vector3.Distance(doorwayContainer.transform.GetChild(i).position, halfwayPoint) < Vector3.Distance(closestDoor.transform.position, halfwayPoint))
            {
                closestDoor = doorwayContainer.transform.GetChild(i).gameObject;
            }
        }
        return closestDoor;

        /*GameObject[] allDoors = new GameObject[doorwayContainer.transform.childCount];
        for (int i = 0; i < allDoors.Length; i++)
        {
            allDoors[i] = doorwayContainer.transform.GetChild(i).gameObject;
        }

        allDoors.OrderByDescending(x => Vector3.Distance(x.transform.position, halfwayPoint)).ToArray();
        foreach(GameObject door in allDoors)
        {
            Debug.Log(Vector3.Distance(door.transform.position, halfwayPoint));
        }
        return allDoors;*/
    }
}
