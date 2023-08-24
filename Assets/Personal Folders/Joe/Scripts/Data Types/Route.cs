using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Route : Edge
{
    public Branch nextBranch { get; private set; }

    public Route(Vertex v1, Vertex v2, Branch nextBranch):base(v1, v2)
    {
        this.nextBranch = nextBranch;
    }

    public Route(Route r)
    {
        v1 = r.v1;
        v2 = r.v2;

        nextBranch = r.nextBranch;
    }

    public Route() : base()
    {
        
    }
}
