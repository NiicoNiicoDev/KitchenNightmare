using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class used for storing 2 vertices that make up an Edge
/// </summary>
public class Edge
{
    public Vertex v1;
    public Vertex v2;

    public Vertex opposite;

    public Edge nextEdge;
    public Edge previousEdge;

    /// <summary>
    /// Default constructor
    /// </summary>
    public Edge()
    {
        v1 = new Vertex();
        v2 = new Vertex();
        opposite = new Vertex();
    }

    public Edge(Vector2 start, Vector2 end)
    {
        v1.x = start.x;
        v1.y = start.y;

        v2.x = end.x;
        v2.y = end.y;
    }

    /// <summary>
    /// Overloaded constructor that takes in 2 vertices
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    public Edge(Vertex v1, Vertex v2)
    {
        this.v1 = v1;
        this.v2 = v2;
        opposite = new Vertex();
    }

    /// <summary>
    /// Overloaded constructor that takes in an Edge
    /// </summary>
    /// <param name="e"></param>
    public Edge(Edge e)
    {
        v1 = e.v1;
        v2 = e.v2;
        opposite = e.opposite;
        nextEdge = e.nextEdge;
        previousEdge = e.previousEdge;
    }

    //Code adapted from Jesse (2008)
    /// <summary>
    /// Returns true if the 2 edges are intersecting
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool IsIntersecting(Edge other)
    {
        //Code inspired by Jesse (2008)
        bool intersecting = false;

        //First work out denomiator of line intersection equation
        float denominator = (other.v2.y - other.v1.y) * (v2.x - v1.x) - (other.v2.x - other.v1.x) * (v2.y - v1.y);

        //If denominator is 0 the lines are parallel and therefore cannot intersect
        if (denominator == 0)
        {
            intersecting = false;
        }
        else
        {
            //Since non parallel lines will always intersect at some point - find the point of intersection between lines
            float unknown1 = ((other.v2.x - other.v1.x) * (v1.y - other.v1.y) - (other.v2.y - other.v1.y) * (v1.x - other.v1.x)) / denominator;
            float unknown2 = ((v2.x - v1.x) * (v1.y - other.v1.y) - (v2.y - v1.y) * (v1.x - other.v1.x)) / denominator;

            //If the intersection is within the original line segements then they are intersecting
            if (unknown1 > 0 && unknown1 < 1 && unknown2 > 0 && unknown2 < 1)
            {
                intersecting = true;
            }
        }

        return intersecting;
    }
    //End of adapted code

    /// <summary>
    /// Returns a list of all edges that are duplicates in a given list
    /// </summary>
    /// <param name="edges"></param>
    /// <returns></returns>
    public static List<Edge> FindCommonEdges(List<Edge> edges)
    {
        //Creates a temp list that stores the same as edges
        List<Edge> tempList = new List<Edge>(edges);
        List<Edge> commonEdges = new List<Edge>();

        //Loop through all edges
        for (int i = 0; i < edges.Count; i++)
        {
            Edge currentEdge = edges[i];

            //Creates a separate list that stores all found duplicates
            List<Edge> duplicateEdges = new List<Edge>();

            //Loop through tempList and add all duplicates to duplicateEdges
            for (int j = 0; j < tempList.Count; j++)
            {
                if (currentEdge == tempList[j])
                {
                    duplicateEdges.Add(tempList[j]);
                }
            }
            //Adds first instance of a duplicate to the common edges list
            if (duplicateEdges.Count > 1)
            {
                commonEdges.Add(new Edge(currentEdge));
            }

            //Loop through and remove all duplicate edges from temp list to reduce next loop size
            for (int n = 0; n < duplicateEdges.Count; n++)
            {
                //Ensures the common edge has access to the line opposite
                if (duplicateEdges.Count > 1)
                {
                    Edge e = commonEdges[commonEdges.Count - 1];

                    if ((e.nextEdge != duplicateEdges[n].nextEdge && e.previousEdge != duplicateEdges[n].previousEdge) && (e.nextEdge != duplicateEdges[n].previousEdge && e.previousEdge != duplicateEdges[n].nextEdge))
                    {
                        if (duplicateEdges[n].nextEdge.v2 != new Vertex())
                        {
                            e.opposite = duplicateEdges[n].nextEdge.v2;
                        }
                    }
                }

                tempList.Remove(duplicateEdges[n]);
            }
        }

        return commonEdges;
    }

    /// <summary>
    /// Returns a copy of an edge list
    /// </summary>
    /// <param name="edges"></param>
    /// <returns></returns>
    public static List<Edge> CloneEdges(List<Edge> edges)
    {
        List<Edge> newList = new List<Edge>();

        for (int i = 0; i < edges.Count; i++)
        {
            newList.Add(new Edge(edges[i]));
        }

        return newList;
    }

    /// <summary>
    /// Performs an edge flip
    /// </summary>
    public void FlipEdge()
    {
        v1 = nextEdge.v2;
        v2 = opposite;
    }

    public static bool operator ==(Edge lhs, Edge rhs)
    {
        if (lhs.v1 == rhs.v1 && lhs.v2 == rhs.v2)
        {
            return true;
        }
        else if (lhs.v1 == rhs.v2 && lhs.v2 == rhs.v1)
        {
            return true;
        }


        if (lhs.v1.x == rhs.v1.x && lhs.v1.y == rhs.v1.y && lhs.v2.x == rhs.v2.x && lhs.v2.y == rhs.v2.y)
        {
            return true;
        }

        return false;
    }

    public static bool operator !=(Edge lhs, Edge rhs)
    {
        if (lhs.v1 == rhs.v1 && lhs.v2 == rhs.v2)
        {
            return false;
        }
        else if (lhs.v1 == rhs.v2 && lhs.v2 == rhs.v1)
        {
            return false;
        }


        if (lhs.v1.x == rhs.v1.x && lhs.v1.y == rhs.v1.y && lhs.v2.x == rhs.v2.x && lhs.v2.y == rhs.v2.y)
        {
            return false;
        }

        return true;
    }

    public float sqrMagnitude
    {
        get 
        { 
            Vector2 v = new Vector2(v2.x - v1.x, v2.y - v1.y);
            return v.sqrMagnitude;
        }
    }

    public string String
    {
        get
        {
            return "[" + v1.x + ", " + v1.y + "] [" + v2.x + ", " + v2.y + "]";
        }
    }

}
