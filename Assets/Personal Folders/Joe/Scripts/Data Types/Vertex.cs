using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class used for storing the vertices of the centre of each room
/// </summary>
public class Vertex
{
    public float x;
    public float y;

    /// <summary>
    /// Default Constructor
    /// </summary>
    public Vertex()
    {
        x = 0;
        y = 0;
    }

    /// <summary>
    /// Overloaded constructor takes in 2 arguments (y should be set to a transform's z)
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public Vertex(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    /// <summary>
    /// Returns a Vector3 representing the world position of a vertex
    /// </summary>
    public Vector3 WorldPosition
    { 
        get { return new Vector3(x, 0, y); }    
    }

    public static Vertex Zero
    {
        get { return new Vertex(0, 0); }
    }

    public static bool operator ==(Vertex lhs, Vertex rhs)
    {
        if (lhs.x == rhs.x && lhs.y == rhs.y)
        {
            return true;
        }

        return false;
    }

    public static bool operator !=(Vertex lhs, Vertex rhs)
    {
        if (lhs.x != rhs.x || lhs.y != rhs.y)
        {
            return true;
        }

        return false;
    }

    public static Vector2 operator -(Vertex lhs, Vertex rhs)
    {
        Vector2 result = new Vector2(lhs.x - rhs.x, lhs.y - rhs.y);
        return result;
    }
}
