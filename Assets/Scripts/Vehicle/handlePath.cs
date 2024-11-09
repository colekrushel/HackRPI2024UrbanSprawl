using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class handlePath : Vehicle
{
    //// Start is called before the first frame update

    public override void Start()
    {
        base.Start();
        GeneratePath();

    }

    //// Update is called once per frame
    //void Update()
    //{

    //}


    //generate path based on curr pos


    void GeneratePath()

    {
        bool pathFinished = false;
        path = new List<Vector3>();
        //generate a path starting from the first tile and going to every tile visitable after that
        //make a random decision at branches

        while (!pathFinished) //loop until an end is reached
        {
            print(currTilePos);
            //get tile vehicle is currently in (last element of path
            Tile T = myTilemap.GetTile<Tile>(tilePath[0]);
            //get next tile, discern by name
            print(T.name);
            print(path[0]);

            //handle invalid path cases

            //edge of map

            //unfilled road


            Vector3Int newTilePos;
            Tile T2;

            switch (T.name)
            {
                
                case "RIGHTLANE":
                    newTilePos = tilePath[0] + new Vector3Int(1, 0, 0);
                    //get right tile
                    T2 = myTilemap.GetTile<Tile>(newTilePos);
                    //add to path
                    tilePath.Add(newTilePos);
                    path.Add(myTilemap.CellToWorld(newTilePos));
                    break;

                case "LEFTLANE":
                    newTilePos = tilePath[0] + new Vector3Int(-1, 0, 0);
                    //get left tile
                    T2 = myTilemap.GetTile<Tile>(newTilePos);
                    //add to path
                    tilePath.Add(newTilePos);
                    path.Add(myTilemap.CellToWorld(newTilePos));
                    break;

                case "DOWNLANE:":
                    newTilePos = tilePath[0] + new Vector3Int(0, -1, 0);
                    //get below tile
                    T2 = myTilemap.GetTile<Tile>(newTilePos);
                    //add to path
                    tilePath.Add(newTilePos);
                    path.Add(myTilemap.CellToWorld(newTilePos));
                    break;

                case "UPLANE":
                    newTilePos = tilePath[0] + new Vector3Int(0, 1, 0);
                    //get right tile
                    T2 = myTilemap.GetTile<Tile>(newTilePos);
                    //add to path
                    tilePath.Add(newTilePos);
                    path.Add(myTilemap.CellToWorld(newTilePos));
                    break;

                default:
                    break;
            }

            pathFinished = true;
        }
        
    }
}
