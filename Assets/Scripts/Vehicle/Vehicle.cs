using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Vehicle : MonoBehaviour
{
    protected bool spinOut;
    protected Vector3Int currTilePos;
    protected Vector3 nextDestination;
    [SerializeField] protected Tilemap myTilemap;
    protected List<Vector3> path;
    protected List<Vector3Int> tilePath;
    protected ParticleSystem explosionEffect;
    // Start is called before the first frame update
    virtual public void Start()
    {
        spinOut = false;
        myTilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        explosionEffect = gameObject.GetComponent<ParticleSystem>();
        explosionEffect.Stop();
        //set vehicle pos relative to start pos
        //get tile vehicle is currently "on"
        Vector3Int currTilePos = myTilemap.WorldToCell(gameObject.transform.position);
        //add current tile (start tile) to path
        Tile T = myTilemap.GetTile<Tile>(currTilePos);

        tilePath = new List<Vector3Int>();
        tilePath.Add(currTilePos);
        Vector3 tilePos = myTilemap.CellToWorld(currTilePos);
        path = new List<Vector3>();
        path.Add(tilePos + new Vector3(.5f, .5f, 0));
        //generate path for object based on tilemap data
        //Tilemap = GameObject.Find("Grid");
    }
}

