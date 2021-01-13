using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class navagationBaker : MonoBehaviour
{
    //public NavMeshSurface[] surfaces;
    private NavMeshSurface objMesh;
    // Start is called before the first frame update
    void OnEnable()
    {
        objMesh = GetComponent<NavMeshSurface>();
        // for( int i = 0; i < surfaces.Length; i++)
        //{
        // surfaces[i].BuildNavMesh();
        // }
        objMesh.BuildNavMesh();
    }
}
