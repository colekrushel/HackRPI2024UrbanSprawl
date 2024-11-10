using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
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
    void Update()
    {
        //if spinning out stop moving
        if (spinOut)
        {
            //stop
        }
        else
        {


            //animate vehicle along path
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0);
            Vector3 nextPos = path[0];
            Vector3 rotationDir = nextPos - gameObject.transform.position;
            //animate vehicle towards first element in path
            Vector3 movementDirection = Vector3.MoveTowards(gameObject.transform.position, nextPos, 2f * Time.deltaTime);
            gameObject.transform.position = movementDirection;
            if (rotationDir != Vector3.zero)
            {
                gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, Quaternion.LookRotation(rotationDir), 20f * Time.deltaTime);
            }
            //gameObject.transform.forward = rotationDir;
            //if vehicle reaches this element, then cut it from the path
            if (Vector3.Distance(gameObject.transform.position, nextPos) <= 0.01f)
            {
                //if this is the last path element, then don't remove it
                if(nextPos == path[path.Count - 1])
                {
                    //check if player is on an exit tile
                    Tile T = myTilemap.GetTile<Tile>(myTilemap.WorldToCell(gameObject.transform.position));
                    if (T.name == "ExitUp" || T.name == "ExitDown" || T.name == "ExitLeft" || T.name == "ExitRight"){
                        //kill
                        Destroy(gameObject);
                    }

                } else
                {
                    path.RemoveAt(0);
                }
                
            }
            //if (path.Count == 0)
            //{
            //    //if no more items left in path, have vehicle wait
            //    StartCoroutine(killself());
            //}
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        print("EXPLODE!!!");
        //explode
        explosionEffect.Play();
        gameObject.GetComponent<Rigidbody>().AddForce(other.gameObject.transform.forward * 50);
        new WaitForSeconds(2);
        //delete object
        StartCoroutine(killself());

    }

    IEnumerator killself()
    {
        spinOut = true;
        //wait for 2 seconds
        yield return new WaitForSeconds(1);
        //kill self
        Destroy(gameObject);

    }



    private void resetPath()
    {
        //set vehicle pos relative to start pos
        //get tile vehicle is currently "on"
        Vector3Int currTilePos = myTilemap.WorldToCell(gameObject.transform.position);
        //add current tile (start tile) to path
        Tile T = myTilemap.GetTile<Tile>(currTilePos);

        tilePath = new List<Vector3Int>();
        tilePath.Add(currTilePos);
        Vector3 tilePos = myTilemap.CellToWorld(currTilePos);
        path = new List<Vector3>();
        //path.Add(tilePos + new Vector3(.5f, .5f, 0));
    }


    //generate path based on curr pos
    public void GeneratePath()

    {
        resetPath();
        int c = 1;
        bool pathFinished = false;
        //generate a path starting from the first tile and going to every tile visitable after that
        //make a random decision at branches

        while (!pathFinished) //loop until an end is reached
        {
            c += 1;
            if (c == 500) { pathFinished = true; break; }
            //print(currTilePos);
            //get last elemnt in tile path
            Tile T = myTilemap.GetTile<Tile>(tilePath[tilePath.Count-1]);
            print(T.name);
            if (T == null)
            {
                //break loop if tile is null
                pathFinished = true;
                break;
            }
            //print("path loop");
            //print(T.name);
            //print("path pos:");
            //print(path[path.Count-1]);

            //handle illegal roads?



            Vector3Int newTilePos;
            Tile T2;

            switch (T.name)
            {
                case "Blank":
                    //on blank tile, keep going prev direction
                case "StartLeft":
                case "LaneRight":

                    newTilePos = tilePath[tilePath.Count - 1] + new Vector3Int(1, 0, 0);
                    //get right tile
                    T2 = myTilemap.GetTile<Tile>(newTilePos);
                    //if null end path
                    if(T2 == null)
                    {
                        pathFinished=true;
                        break;
                    }
                    //if blank then skip to next tile
                    while(T2.name == "Blank")
                    {
                        newTilePos = newTilePos + new Vector3Int(1, 0, 0);
                        //get right tile
                        T2 = myTilemap.GetTile<Tile>(newTilePos);
                    }
                    //add to path
                    tilePath.Add(newTilePos);
                    path.Add(myTilemap.CellToWorld(newTilePos) + new Vector3(0.5f, 0.5f, 0));
                    break;
                case "StartRight":
                case "LaneLeft":

                    newTilePos = tilePath[tilePath.Count - 1] + new Vector3Int(-1, 0, 0);
                    //get left tile
                    T2 = myTilemap.GetTile<Tile>(newTilePos);
                    if (T2 == null)
                    {
                        pathFinished = true;
                        break;
                    }
                    //if blank then skip to next tile
                    while (T2.name == "Blank")
                    {
                        newTilePos = newTilePos + new Vector3Int(-1, 0, 0);
                        //get right tile
                        T2 = myTilemap.GetTile<Tile>(newTilePos);
                    }
                    //add to path
                    tilePath.Add(newTilePos);
                    path.Add(myTilemap.CellToWorld(newTilePos) + new Vector3(0.5f, 0.5f, 0));
                    break;
                case "StartDown":
                case "LaneDown":

                    newTilePos = tilePath[tilePath.Count - 1] + new Vector3Int(0, -1, 0);
                    //get below tile
                    T2 = myTilemap.GetTile<Tile>(newTilePos);
                    if (T2 == null)
                    {
                        pathFinished = true;
                        break;
                    }
                    //if blank then skip to next tile
                    while (T2.name == "Blank")
                    {
                        newTilePos = newTilePos + new Vector3Int(0, -1, 0);
                        //get right tile
                        T2 = myTilemap.GetTile<Tile>(newTilePos);
                    }
                    //add to path
                    tilePath.Add(newTilePos);
                    path.Add(myTilemap.CellToWorld(newTilePos) + new Vector3(0.5f, 0.5f, 0));
                    break;
                case "StartUp":
                case "LaneUp":

                    newTilePos = tilePath[tilePath.Count - 1] + new Vector3Int(0, 1, 0);
                    //get above tile
                    T2 = myTilemap.GetTile<Tile>(newTilePos);
                    if (T2 == null)
                    {
                        pathFinished = true;
                        break;
                    }
                    //if blank then skip to next tile
                    while (T2.name == "Blank")
                    {
                        newTilePos = newTilePos + new Vector3Int(0, 1, 0);
                        //get right tile
                        T2 = myTilemap.GetTile<Tile>(newTilePos);
                    }
                    //add to path
                    tilePath.Add(newTilePos);
                    path.Add(myTilemap.CellToWorld(newTilePos) + new Vector3(0.5f, 0.5f, 0));
                    break;

                case "CornerRightToDown": //handle corners
                    newTilePos = tilePath[tilePath.Count - 1] + new Vector3Int(-1, 0, 0); //left tile
                    //if tile is already in path then go other way
                    if (tilePath.Contains(newTilePos))
                    {
                        newTilePos = tilePath[tilePath.Count - 1] + new Vector3Int(0, -1, 0); //down tile
                        //get down tile
                        T2 = myTilemap.GetTile<Tile>(newTilePos);
                        //if blank then skip to next tile
                        while (T2.name == "Blank")
                        {
                            newTilePos = newTilePos + new Vector3Int(0, -1, 0);
                            //get down tile
                            T2 = myTilemap.GetTile<Tile>(newTilePos);
                        }
                    }
                    else
                    {
                        //get left tile
                        T2 = myTilemap.GetTile<Tile>(newTilePos);
                        //if blank then skip to next tile
                        while (T2.name == "Blank")
                        {
                            newTilePos = newTilePos + new Vector3Int(-1, 0, 0);
                            //get left tile
                            T2 = myTilemap.GetTile<Tile>(newTilePos);
                        }
                    }
                    //add to path
                    tilePath.Add(newTilePos);
                    path.Add(myTilemap.CellToWorld(newTilePos) + new Vector3(0.5f, 0.5f, 0));
                    break;
                
                case "CornerDownToLeft":
                    newTilePos = tilePath[tilePath.Count - 1] + new Vector3Int(-1, 0, 0); //left tile
                    //if tile is already in path then go other way
                    if (tilePath.Contains(newTilePos))
                    {
                        newTilePos = tilePath[tilePath.Count - 1] + new Vector3Int(0, 1, 0); //up tile
                        //get up tile
                        T2 = myTilemap.GetTile<Tile>(newTilePos);
                        //if blank then skip to next tile
                        while (T2.name == "Blank")
                        {
                            newTilePos = newTilePos + new Vector3Int(0, 1, 0);
                            //get up tile
                            T2 = myTilemap.GetTile<Tile>(newTilePos);
                        }
                    }
                    else
                    {
                        //get left tile
                        T2 = myTilemap.GetTile<Tile>(newTilePos);
                        //if blank then skip to next tile
                        while (T2.name == "Blank")
                        {
                            newTilePos = newTilePos + new Vector3Int(-1, 0, 0);
                            //get left tile
                            T2 = myTilemap.GetTile<Tile>(newTilePos);
                        }
                    }
                    //add to path
                    tilePath.Add(newTilePos);
                    path.Add(myTilemap.CellToWorld(newTilePos) + new Vector3(0.5f, 0.5f, 0));
                    break;
                case "CornerLeftToUp":
                    newTilePos = tilePath[tilePath.Count - 1] + new Vector3Int(1, 0, 0); //right tile
                    //if tile is already in path then go other way
                    if (tilePath.Contains(newTilePos))
                    {
                        newTilePos = tilePath[tilePath.Count - 1] + new Vector3Int(0, 1, 0); //up tile
                        //get up tile
                        T2 = myTilemap.GetTile<Tile>(newTilePos);
                        //if blank then skip to next tile
                        while (T2.name == "Blank")
                        {
                            newTilePos = newTilePos + new Vector3Int(0, 1, 0);
                            //get right tile
                            T2 = myTilemap.GetTile<Tile>(newTilePos);
                        }
                    }
                    else
                    {
                        //get right tile
                        T2 = myTilemap.GetTile<Tile>(newTilePos);
                        //if blank then skip to next tile
                        while (T2.name == "Blank")
                        {
                            newTilePos = newTilePos + new Vector3Int(1, 0, 0);
                            //get right tile
                            T2 = myTilemap.GetTile<Tile>(newTilePos);
                        }
                    }
                    //add to path
                    tilePath.Add(newTilePos);
                    path.Add(myTilemap.CellToWorld(newTilePos) + new Vector3(0.5f, 0.5f, 0));
                    break;
                case "CornerUpToRight":
                    newTilePos = tilePath[tilePath.Count - 1] + new Vector3Int(1, 0, 0); //right tile
                    //if tile is already in path then go other way
                    if (tilePath.Contains(newTilePos))
                    {
                        newTilePos = tilePath[tilePath.Count - 1] + new Vector3Int(0, -1, 0); //down tile
                        //get down tile
                        T2 = myTilemap.GetTile<Tile>(newTilePos);
                        //if blank then skip to next tile
                        while (T2.name == "Blank")
                        {
                            newTilePos = newTilePos + new Vector3Int(0, -1, 0);
                            //get down tile
                            T2 = myTilemap.GetTile<Tile>(newTilePos);
                        }
                    }
                    else
                    {
                        //get right tile
                        T2 = myTilemap.GetTile<Tile>(newTilePos);
                        //if blank then skip to next tile
                        while (T2.name == "Blank")
                        {
                            newTilePos = newTilePos + new Vector3Int(1, 0, 0);
                            //get right tile
                            T2 = myTilemap.GetTile<Tile>(newTilePos);
                        }
                    }
                    //add to path
                    tilePath.Add(newTilePos);
                    path.Add(myTilemap.CellToWorld(newTilePos) + new Vector3(0.5f, 0.5f, 0));
                    break;
                case "ExitUp":
                case "ExitDown":
                case "ExitRight":
                case "ExitLeft":
                    //end loop 
                    pathFinished = true;
                    //Destroy(gameObject);
                    break;
                default:
                    break;
            }

            //pathFinished = true;
        }
        
    }
}
