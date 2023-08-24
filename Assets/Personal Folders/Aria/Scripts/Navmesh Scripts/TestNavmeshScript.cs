using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

[RequireComponent(typeof(NavMeshSurface))]
public class TestNavmeshScript : MonoBehaviour
{
    NavMeshSurface navMeshSurface;
    NavMeshData navMeshData;

    // Start is called before the first frame update
    void Start()
    {
        GenerateNavMesh();
        //navMeshData = navMeshSurface.navMeshData;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateNavMesh()
    {
        navMeshSurface = GetComponent<NavMeshSurface>();
        navMeshSurface.BuildNavMesh();
    }
}
