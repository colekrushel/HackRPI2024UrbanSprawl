using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animateVehicle : Vehicle
{
    // Start is called before the first frame update
    //void Start()
    //{
    //    //uhm do nothin?
    //}

    // Update is called once per frame
    void Update()
    {
        Vector3 nextPos = path[0];

        //animate vehicle towards first element in path
        Vector3.MoveTowards(gameObject.transform.position, nextPos, 0.01f);
        //if vehicle reaches this element, then cut it from the path
        if (Vector3.Distance(gameObject.transform.position, nextPos) < 0.01f)
        {
            path.RemoveAt(0);
        }
        if (path.Count == 0)
        {
            //if no more items left in path, remove this vehicle (or crash it?)
            gameObject.SetActive(false);
        }
    }
}
