using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class handlePath : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 nextDestination;
    List<Vector3> path; 
    void Start()
    {
        //generate path for object based on tilemap data

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GeneratePath()
    {
        path = new List<Vector3>();
        //generate a path starting from the first tile and going to every tile visitable after that
        //make a random decision at branches
        
    }
}
