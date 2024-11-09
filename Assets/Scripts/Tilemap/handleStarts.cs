using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class handleStarts : MonoBehaviour
{
    [SerializeField] GameObject vehiclePrefab;
    [SerializeField] Tilemap tilemap;
    List<Vector3Int> startTiles;
    float spawnDelay;
    float spawnTime;
    // Start is called before the first frame update
    void Start()
    {
        startTiles = new List<Vector3Int>();
        spawnDelay = 4f;
        spawnTime = 4f;
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
                            startTiles.Add(new Vector3Int(x, y, z));
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
            for (int i = 0; i < startTiles.Count; i++)
            {

                GameObject newCar = Instantiate(vehiclePrefab, startTiles[i] + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
            }
            spawnTime = 0;
        }
        //print(spawnTime);
        spawnTime += Time.deltaTime;
    }
}
