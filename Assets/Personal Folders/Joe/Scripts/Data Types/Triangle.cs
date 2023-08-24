using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class used for storing 3 vertices and the edges connecting them
/// </summary>
public class Triangle
{
    Vertex v1;
    Vertex v2;
    Vertex v3;

    public Edge e1;
    public Edge e2;
    public Edge e3;

    /// <summary>
    /// Overloaded constructor takes in 3 vertices
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <param name="v3"></param>
    public Triangle(Vertex v1, Vertex v2, Vertex v3)
    {
        this.v1 = v1;
        this.v2 = v2;
        this.v3 = v3;

        //Next edges v1 will always be edges v2
        e1 = new Edge(v1, v2);
        e2 = new Edge(v2, v3);
        e3 = new Edge(v3, v1);

        SetAdjacentEdges();
    }

    /// <summary>
    /// Overloaded constructor takes in 3 edges
    /// </summary>
    /// <param name="e1"></param>
    /// <param name="e2"></param>
    /// <param name="e3"></param>
    public Triangle(Edge e1, Edge e2, Edge e3)
    {
        this.e1 = e1;
        this.e2 = e2;
        this.e3 = e3;

        v1 = e1.v1;
        v2 = e1.v2;
        v3 = e2.v2;

        SetAdjacentEdges();
    }

    private void SetAdjacentEdges()
    {
        e1.nextEdge = e2;
        e2.nextEdge = e3;
        e3.nextEdge = e1;

        e1.previousEdge = e3;
        e2.previousEdge = e1;
        e3.previousEdge = e2;
    }

    public void ReOrder()
    {
        Vertex temp = v2;
        v2 = v3;
        v3 = temp;
    }


    //Code adapted from Nordeus (n.d.)
    public static bool IsQuadrilateralConvex(Vertex a, Vertex b, Vertex c, Vertex d)
    {
        bool abc = IsTriangleClockwise(new Triangle(a, b, c));
        bool abd = IsTriangleClockwise(new Triangle(a, b, d));
        bool bcd = IsTriangleClockwise(new Triangle(b, c, d));
        bool cad = IsTriangleClockwise(new Triangle(c, a, d));

        if (abc && abd && bcd & !cad)
        {
            return true;
        }
        else if (abc && abd && !bcd & cad)
        {
            return true;
        }
        else if (abc && !abd && bcd & cad)
        {
            return true;
        }
        else if (!abc && !abd && !bcd & cad)
        {
            return true;
        }
        else if (!abc && !abd && bcd & !cad)
        {
            return true;
        }
        else if (!abc && abd && !bcd & !cad)
        {
            return true;
        }

        return false;
    }

    public static bool IsTriangleClockwise(Triangle t)
    {
        float determinant = t.v1.x * (t.v2.y - t.v3.y) - t.v1.y * (t.v2.x - t.v3.x) + (t.v2.x * t.v3.y) - (t.v2.y * t.v3.x);

        if (determinant > 0)
        {
            return true;
        }
        return false;
    }

    public static float QuadrilateralDeterminant(Vertex aVec, Vertex bVec, Vertex cVec, Vertex dVec)
    {
        try
        {
            float a = aVec.x - dVec.x;
            float d = bVec.x - dVec.x;
            float g = cVec.x - dVec.x;

            float b = aVec.y - dVec.y;
            float e = bVec.y - dVec.y;
            float h = cVec.y - dVec.y;

            float c = a * a + b * b;
            float f = d * d + e * e;
            float i = g * g + h * h;

            float determinant = (a * e * i) + (b * f * g) + (c * d * h) - (g * e * c) - (h * f * a) - (i * d * b);

            return determinant;
        }
        catch(System.NullReferenceException)
        {
            return 1f;
        }

    }
    //End of adapted code
}
