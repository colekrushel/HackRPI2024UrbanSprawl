using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class handleStarts : MonoBehaviour
{
    [SerializeField] GameObject carPrefab;
    [SerializeField] GameObject busPrefab;
    [SerializeField] Tilemap tilemap;
    List<Vector3Int> carStartTiles;
    List<Vector3Int> busStartTiles;
    float spawnDelay;
    float spawnTime;
    // Start is called before the first frame update
    void Start()
    {
        carStartTiles = new List<Vector3Int>();
        busStartTiles = new List<Vector3Int>();
        spawnDelay = 4f;
        spawnTime = 4f;
    }

    public void resetStarts()
    {
        carStartTiles = new List<Vector3Int>();
        busStartTiles = new List<Vector3Int>();
    }
    public void getStarts()
    {
        carStartTiles = new List<Vector3Int>();
        busStartTiles = new List<Vector3Int>();
        //find locations of all start tiles
        int x, y, z;
        for (x = tilemap.cellBounds.min.x; x < tilemap.cellBounds.max.x; x++)
        {
            for (y = tilemap.cellBounds.min.y; y < tilemap.cellBounds.max.y; y++)
            {
                for (z = tilemap.cellBounds.min.z; z < tilemap.cellBounds.max.z; z++)
                {

                    Tile T = tilemap.GetTile<Tile>(new Vector3Int(x, y, z));
                    if (T != null)
                    {
                        if (T.name == "StartUp" || T.name == "StartDown" || T.name == "StartLeft" || T.name == "StartRight")
                        {
                            carStartTiles.Add(new Vector3Int(x, y, z));
                        }
                        if (T.name == "BusStartUp" || T.name == "BusStartDown" || T.name == "BusStartLeft" || T.name == "BusStartRight")
                        {
                            busStartTiles.Add(new Vector3Int(x, y, z));
                        }
                    }
                }
            }

        }
        
    }
    // Update is called once per frame
    void Update()
    {
        if(spawnTime < (spawnDelay + 1) && spawnTime > (spawnDelay - 1))
        {
            //creeate a vehicle from each spawn tile
            for (int i = 0; i < carStartTiles.Count; i++)
            {
                GameObject newCar = Instantiate(carPrefab, carStartTiles[i] + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
            }
            for (int i = 0; i < busStartTiles.Count; i++)
            {
                GameObject newBus = Instantiate(busPrefab, busStartTiles[i] + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
            }
            spawnTime = 0;
        }
        //print(spawnTime);
        spawnTime += Time.deltaTime;
    }
}
