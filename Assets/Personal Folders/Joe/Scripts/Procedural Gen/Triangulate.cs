using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Triangulate
{
    //Code adapted from Nordeus (n.d.)
    /// <summary>
    /// Uses an incremental algorithm to triangulate vertices (vertices must be ordered by x or y)
    /// </summary>
    /// <param name="vertices"></param>
    /// <returns></returns>
    public static List<Triangle> Incremental(Vertex[] vertices)
    {
        List<Triangle> triangles = new List<Triangle>();

        //List to be returned containing all "perfect" edges
        List<Edge> edges = new List<Edge>();

        //Creates a triangle using the first 3 vertices in the list
        Triangle t = new Triangle(vertices[0], vertices[1], vertices[2]);

        //Adds the triangle's edges into the "perfect" list
        edges.Add(t.e1);
        edges.Add(t.e2);
        edges.Add(t.e3);

        triangles.Add(t);

        //Creates a temporary list used in the final loop - ensures the program doesn't continue to increase its loop condition
        List<Edge> tempEdgeList = new List<Edge>();

        //Every vertex needs an edge to 2 others
        for (int i = 3; i < vertices.Length; i++)
        {
            //The current vertex that needs connecting
            Vertex currentPoint = vertices[i];

            //All edges currently within the "perfect" list
            for (int j = 0; j < edges.Count; j++)
            {
                Edge currentEdge = edges[j];

                //Creates a point in the middle of an edge - this is used to check if an edge can be made without intersection
                Vector3 midpoint = (currentEdge.v1.WorldPosition + currentEdge.v2.WorldPosition) / 2;

                //A test edge is made from the midpoint towards the current vertex being connected
                Vertex v = new Vertex(midpoint.x, midpoint.z);
                Edge testEdge = new Edge(currentPoint, v);

                bool intersecting = false;

                //Loop through all existing "perfect" edges
                for (int k = 0; k < edges.Count; k++)
                {
                    //Skips currentEdge as testEdge will always intersect with it
                    if (edges[j] == edges[k])
                    {
                        continue;
                    }
                    //If testEdge intersects with any other edge - break
                    if (testEdge.IsIntersecting(edges[k]))
                    {
                        intersecting = true;
                        break;
                    }
                }
                //If testEdge does not intersect with any other edges - create a new triangle using currentEdge towards current vertex that needs connecting
                if (intersecting == false)
                {
                    Triangle newT = new Triangle(currentEdge.v1, currentEdge.v2, currentPoint);
                    triangles.Add(newT);

                    tempEdgeList.Add(newT.e1);
                    tempEdgeList.Add(newT.e2);
                    tempEdgeList.Add(newT.e3);
                }
                //Debugging used to ensure Unity doesn't freeze in an infinite loop
                if (j >= 1000)
                {
                    throw new System.ArgumentException("Cannot escape loop");
                }
            }

            //Adds all temp edges that have been made to the current vertex to the "perfect" edge list
            for (int j = 0; j < tempEdgeList.Count; j++)
            {
                edges.Add(tempEdgeList[j]);
            }

            tempEdgeList.Clear();
        }

        return triangles;
    }

    /// <summary>
    /// Returns a list of edges that has passed the delaunay condition
    /// </summary>
    /// <param name="edges"></param>
    /// <returns></returns>
    public static List<Edge> Delaunay(List<Edge> edges)
    {
        //Clones the edges to ensure original list is not altered
        List<Edge> delaunayEdges = Edge.CloneEdges(edges);

        //Creates a list of flipped edges
        List<Edge> flippedEdges = new List<Edge>();

        //Loops through all delaunay edges
        for (int i = 0; i < delaunayEdges.Count; i++)
        {
            //Creates 4 vertices
            Vertex a = delaunayEdges[i].v1;
            Vertex b = delaunayEdges[i].v2;
            Vertex c = delaunayEdges[i].nextEdge.v2;
            Vertex d = delaunayEdges[i].opposite;

            if (d == Vertex.Zero)
            {
                continue;
            }

            //If determinant is < 0 the edge should be flipped
            if (Triangle.QuadrilateralDeterminant(a, b, c, d) < 0f)
            {
                //Ensures the 2 triangles are not concave
                if (Triangle.IsQuadrilateralConvex(a, b, c, d))
                {
                    //Creates flipped edge
                    Edge e = new Edge(delaunayEdges[i].nextEdge.v2, delaunayEdges[i].opposite);

                    //If the newly created edge is longer continue
                    if (e.sqrMagnitude > delaunayEdges[i].sqrMagnitude)
                    {
                        continue;
                    }
                    //If the new triangle still needs flipping continune
                    if (Triangle.QuadrilateralDeterminant(b, c, d, a) < 0f)
                    {
                        continue;
                    }

                    //Ensures newly flipped edges do not intersect with eachother
                    bool isIntersecting = false;

                    //Loop through flipped edges
                    for (int j = 0; j < flippedEdges.Count; j++)
                    {
                        //If any lines intersect break loop
                        if (e.IsIntersecting(flippedEdges[j]))
                        {
                            isIntersecting = true;
                            break;
                        }
                    }
                    //If flipped edge intersects with another flipped edge continue
                    if (isIntersecting)
                    {
                        continue;
                    }

                    //If all other conditions have passed - flip edge

                    delaunayEdges[i].FlipEdge();
                    flippedEdges.Add(delaunayEdges[i]);
                }
            }
        }
        return delaunayEdges;
    }
    //End of adapted code
}
