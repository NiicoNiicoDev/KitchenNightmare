using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Branch
{
    public Vertex root;

    public List<Route> newPathways;

    public Route shortestPath;

    public List<Route> originalPathways;

    public Branch finalDestination;

    public int connectedRooms;

    public bool doNotPathFind = false;


    public Branch()
    {
        root = new Vertex();
        newPathways = new List<Route>();
        shortestPath = new Route();
        originalPathways = new List<Route>();
        connectedRooms = 0;
    }  
    

    public Branch(Vertex v) : this()
    {
        root = v;
    }

    public Branch(Vertex v, Route r) : this()
    {
        root = v;
        shortestPath = r;
    }

    public Branch(Branch b) : this()
    {
        root = b.root;
        newPathways = b.newPathways;
        shortestPath = b.shortestPath;
        originalPathways = b.originalPathways;
    }

    public Branch(Branch b, Route r) : this()
    {
        root = b.root;
        newPathways = b.newPathways;
        shortestPath = r;
        originalPathways = b.originalPathways;
    }


    public static Branch[] CreateBranchList(Vertex[] vertices, List<Edge> edges)
    {
        List<Branch> branches = new List<Branch>();

        for (int i = 0; i < vertices.Length; i++)
        {
            Branch currentBranch = new Branch(vertices[i]);
            bool branchExists = false;

            foreach (Branch b in branches)
            {
                if (currentBranch.root == b.root)
                {
                    currentBranch = b;
                    branchExists = true;
                    break;
                }
            }
            if (!branchExists)
            {
                branches.Add(currentBranch);
            }


            for (int j = 0; j < edges.Count; j++)
            {
                if (edges[j].v1 != currentBranch.root && edges[j].v2 != currentBranch.root) continue;


                Branch connectingBranch = new Branch();

                if (edges[j].v1 == currentBranch.root)
                {
                    connectingBranch.root = edges[j].v2;
                }
                else if (edges[j].v2 == currentBranch.root)
                {
                    connectingBranch.root = edges[j].v1;
                }

                branchExists = false;
                foreach(Branch b in branches)
                {
                    if (connectingBranch.root == b.root)
                    {
                        connectingBranch = b;
                        branchExists = true;
                        break;
                    }
                }
                if (!branchExists)
                {
                    branches.Add(connectingBranch);
                }

                Route route = new Route(currentBranch.root, connectingBranch.root, connectingBranch);
                currentBranch.newPathways.Add(route);
                currentBranch.originalPathways.Add(new Route(route));
            }
        }

        return branches.ToArray();
    }

    public static List<Route> ReOrderPathways(List<Route> routes)
    {
        List<Route> newRoutes = new List<Route>();
        foreach (Route r in routes)
        {
            newRoutes.Add(new Route(r));
        }

        for (int i = 0; i < newRoutes.Count - 1; i++)
        {
            for (int j = 0; j < newRoutes.Count - i - 1; j++)
            {
                if (newRoutes[j].sqrMagnitude > newRoutes[j+1].sqrMagnitude)
                {
                    Route temp = new Route(newRoutes[j]);
                    newRoutes[j] = newRoutes[j + 1];
                    newRoutes[j + 1] = temp;
                }
            }
        }

        return newRoutes;
    }

    public static int FindIndex(List<Branch> branches, Branch currentBranch)
    {
        for (int i = 0; i < branches.Count; i++)
        {
            if (currentBranch == branches[i])
            {
                return i;
            }
        }
        return -1;
    }

    public static Branch FindMissingBranch(List<Branch> list1, Branch[] list2)
    {
        foreach(Branch branch in list2)
        {
            int counter = 0;
            foreach (Branch j in list1)
            {
                if (branch.root == j.root)
                {
                    counter++;
                }
            }
            if (counter == 0)
            {
                return branch;
            }
        }
        return new Branch();
    }
}
