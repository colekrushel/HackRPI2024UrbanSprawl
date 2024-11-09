using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Vehicle : MonoBehaviour
{

    protected Vector3Int currTilePos;
    protected Vector3 nextDestination;
    [SerializeField] protected Tilemap myTilemap;
    protected List<Vector3> path;
    protected List<Vector3Int> tilePath;
    // Start is called before the first frame update
    virtual public void Start()
    {
        //set vehicle pos relative to start pos
        //get tile vehicle is currently "on"
        Vector3Int currTilePos = myTilemap.WorldToCell(gameObject.transform.position);
        //add current tile (start tile) to path
        Tile T = myTilemap.GetTile<Tile>(currTilePos);
        Vector3 tilePos = myTilemap.CellToWorld(currTilePos);
        path = new List<Vector3>();
        path.Add(tilePos);
        tilePath = new List<Vector3Int>();
        tilePath.Add(currTilePos);
        //generate path for object based on tilemap data
        //Tilemap = GameObject.Find("Grid");
    }

    // Update is called once per frame
    void Update()
    {
        //animate vehicle towards path
    }
}
