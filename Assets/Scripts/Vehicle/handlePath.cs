using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class handlePath : Vehicle
{
    //// Start is called before the first frame update
    float waitTimer;
    float tunnelTimer;
    public override void Start()
    {
        base.Start();
        GeneratePath();
        waitTimer = 0;
        tunnelTimer = -1;
    }

    //// Update is called once per frame
    void Update()
    {
        print(tunnelTimer);
        //stop moving on certain conditions
        if (tunnelTimer > 0)
        {
            gameObject.transform.position = new Vector3(50, 50, 50);
            tunnelTimer -= Time.deltaTime;
        } else if (tunnelTimer > -0.2 && tunnelTimer < 0)
        {
            gameObject.transform.position = path[0];
            tunnelTimer = -1;
        }
        else if (spinOut || waitTimer > 0 || path.Count == 0)
        {
            //stop
            waitTimer -= Time.deltaTime;
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
            //vehicle is on edge of path then count as success
            if(path.Count == 1)
            {
                if (Vector3.Distance(gameObject.transform.position, nextPos) <= 0.5f)
                {
                    Tile T = myTilemap.GetTile<Tile>(myTilemap.WorldToCell(gameObject.transform.position));
                    //make sure bus/car are in right exit
                    if (T.name == "ExitUp" || T.name == "ExitDown" || T.name == "ExitLeft" || T.name == "ExitRight")
                    {
                        //kill
                        //if car then safe
                        if (gameObject.tag == "Car")
                        {
                            Destroy(gameObject);
                        }
                        else
                        {
                            //else explode!!
                            explosionEffect.Play();
                            //delete object
                            StartCoroutine(killself());
                        }
                        
                    } else if(T.name == "BusExitUp" || T.name == "BusExitDown" || T.name == "BusExitLeft" || T.name == "BusExitRight")
                    {
                        //if bus then safe
                        if (gameObject.tag == "Bus")
                        {
                            Destroy(gameObject);
                        }
                        else
                        {
                            //else explode!!
                            explosionEffect.Play();
                            //delete object
                            StartCoroutine(killself());
                        }
                    }
                }
            }
            //if vehicle reaches this element, then cut it from the path
            if (Vector3.Distance(gameObject.transform.position, nextPos) <= 0.01f)
            {
                //if this is a tunnel then activate tunnel timer
                Tile T = myTilemap.GetTile<Tile>(myTilemap.WorldToCell(gameObject.transform.position));
                if(T.name == "TunnelInDown" || T.name == "TunnelInUp" || T.name == "TunnelInRight" || T.name == "TunnelInLeft")
                {
                    tunnelTimer = 1f;
                }
                
                //if path has a stoplight on it, then start a wait period until car moves again
                Tile T2 = topTilemap.GetTile<Tile>(topTilemap.WorldToCell(gameObject.transform.position));
                if(T2 != null)
                {
                    waitTimer = 1f;
                }
                path.RemoveAt(0);
                
                
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
        //print("EXPLODE!!!");
        //explode
        explosionEffect.Play();
        gameObject.GetComponent<Rigidbody>().AddForce(other.gameObject.transform.forward * 50);
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
            if (T == null)
            {
                //break loop if tile is null
                pathFinished = true;
                break;
            }

            //handle illegal roads?


            Vector3 tunnelExit;
            Vector3Int newTilePos;
            Tile T2;

            switch (T.name)
            {
                case "Blank":
                    //on blank tile, keep going prev direction
                    break;
                case "TunnelOutRight":
                case "BusStartRight":
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
                case "TunnelOutLeft":
                case "BusStartLeft":
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
                case "TunnelOutDown":
                case "BusStartDown":
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
                case "TunnelOutUp":
                case "BusStartUp":
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
                //tunnels
                case "TunnelInUp":
                    //get associated exit
                    tunnelExit = getTunnelExit("Up");
                    if (tunnelExit != null)
                    {
                        path.Add(tunnelExit);
                        tilePath.Add(myTilemap.WorldToCell(tunnelExit));
                    }
                    else
                    {
                        pathFinished = true;
                    }
                    break;
                case "TunnelInDown":
                    //get associated exit
                    tunnelExit = getTunnelExit("Down");
                    if (tunnelExit != null)
                    {
                        path.Add(tunnelExit);
                        tilePath.Add(myTilemap.WorldToCell(tunnelExit));
                    }
                    else
                    {
                        pathFinished = true;
                    }
                    break;
                case "TunnelInRight":
                    //get associated exit
                    tunnelExit = getTunnelExit("Right");
                    if (tunnelExit != null)
                    {
                        path.Add(tunnelExit);
                        tilePath.Add(myTilemap.WorldToCell(tunnelExit));
                    }
                    else
                    {
                        pathFinished = true;
                    }
                    break;
                case "TunnelInLeft":
                    //get associated exit
                    tunnelExit = getTunnelExit("Left");
                    if (tunnelExit != null)
                    {
                        path.Add(tunnelExit);
                        tilePath.Add(myTilemap.WorldToCell(tunnelExit));
                    } else
                    {
                        pathFinished = true;
                    }
                    break;
                case "ExitUp":
                case "ExitDown":
                case "ExitRight":
                case "ExitLeft":
                    pathFinished = true;
                    break;
                case "BusExitUp":
                case "BusExitDown":
                case "BusExitRight":
                case "BusExitLeft":

                    pathFinished = true;
                    break;
                default:
                    break;
            }

            //pathFinished = true;
        }
        
    }

    Vector3 getTunnelExit(string d)
    {
        int x, y, z;
        for (x = myTilemap.cellBounds.min.x; x < myTilemap.cellBounds.max.x; x++)
        {
            for (y = myTilemap.cellBounds.min.y; y < myTilemap.cellBounds.max.y; y++)
            {
                for (z = myTilemap.cellBounds.min.z; z < myTilemap.cellBounds.max.z; z++)
                {

                    Tile T = myTilemap.GetTile<Tile>(new Vector3Int(x, y, z));
                    if (T != null)
                    {
                        switch (d)
                        {
                            case "Up":
                                if(T.name == "TunnelOutUp")
                                {
                                    return myTilemap.CellToWorld(new Vector3Int(x, y, z)) + new Vector3(.5f, .5f, 0f);
                                }
                                break;
                            case "Down":
                                if (T.name == "TunnelOutDown")
                                {
                                    return myTilemap.CellToWorld(new Vector3Int(x, y, z)) + new Vector3(.5f, .5f, 0f);
                                }
                                break;
                            case "Left":
                                if (T.name == "TunnelOutLeft")
                                {
                                    return myTilemap.CellToWorld(new Vector3Int(x, y, z)) + new Vector3(.5f, .5f, 0f);
                                }
                                break;
                            case "Right":
                                if (T.name == "TunnelOutRight")
                                {
                                    return myTilemap.CellToWorld(new Vector3Int(x, y, z)) + new Vector3(.5f, .5f, 0f);
                                }
                                break;
                        }
                    }
                   
                }
            }

        }
        return new Vector3(0, 0, 0);

    }
}
