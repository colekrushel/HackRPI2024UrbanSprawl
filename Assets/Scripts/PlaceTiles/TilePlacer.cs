using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TilePlacer : MonoBehaviour
{

    [SerializeField]
    CashHandler cashHandler;

    [SerializeField]
    Vector3 mousTOffset;

    [SerializeField]
    Tilemap placeTileMap, topTileMap;
    [SerializeField]
    Button BlankButton, CornerLeftButton, CornerRightButton, LaneButton, LightsButton, StartButton, ResetButton;
    [SerializeField]
    TileRotator BlankTile, CornerLeftTile, CornerRightTile, LaneTile, LightsTile;

    TileBase tileToPlace;
    int cost;
    SpriteRenderer currentSpriteRenderer;

    Vector3 mousTWrld;
    Vector3Int properlyMousTWrld;

    [SerializeField]
    TileBase[] protectedTiles;

    [SerializeField]
    Tile[] currentTiles;
    int currentTileInt;
    int currentTileIntLength;

    Dictionary<TileBase, int> tileToCost;

    // Start is called before the first frame update
    void Start()
    {
        tileToCost = new Dictionary<TileBase, int>();
        currentSpriteRenderer = GetComponent<SpriteRenderer>();

        BlankButton.onClick.AddListener(delegate { SetTileAndCost(BlankTile); });
        CornerLeftButton.onClick.AddListener(delegate { SetTileAndCost(CornerLeftTile); });
        //CornerRightButton.onClick.AddListener(delegate { SetTileAndCost(CornerRightTile, 5); });
        LaneButton.onClick.AddListener(delegate { SetTileAndCost(LaneTile); });
        LightsButton.onClick.AddListener(delegate { SetTileAndCost(LightsTile); });

        StartButton.onClick.AddListener(delegate { GameObject.Find("Tilemap").GetComponent<handleStarts>().getStarts(); });
        ResetButton.onClick.AddListener(delegate { ResetCanvas(); });

        AddToTileCostDictionairy(BlankTile);
        AddToTileCostDictionairy(CornerLeftTile);
        AddToTileCostDictionairy(LaneTile);
        AddToTileCostDictionairy(LightsTile);


        SetTileAndCost(BlankTile);
    }

    void AddToTileCostDictionairy(TileRotator tileRotator) {
        foreach (Tile item in tileRotator.Tiles) {
            tileToCost.Add(item, tileRotator.TileCost);
        }
    }

    void SetTileAndCost(TileRotator placeTile) {
        Tile curTile = placeTile.CurrentTile;
        tileToPlace = curTile;
        Debug.Log(curTile);
        currentSpriteRenderer.sprite = curTile.sprite;
        cost = placeTile.TileCost;


        //Should have just used an event manager :skull:
        currentTiles = placeTile.Tiles;
        currentTileInt = placeTile.CurrentTileInt;
        currentTileIntLength = placeTile.CurrentTileIntLength;

        placeTileMap = placeTile.PlaceableTilemaps;
    }

    void SetNonProtectedTiles() {
        

        TileBase tileWeWantToReplace = placeTileMap.GetTile(properlyMousTWrld);

        if (tileWeWantToReplace == tileToPlace) { return; } //Don't want to place the same tile ontop of the same one lol
        if (tileWeWantToReplace != null)
        {
            foreach (TileBase protectedTile in protectedTiles)
            {
                if (protectedTile == tileWeWantToReplace) {
                    return;
                }
            }

            if (tileToCost[tileWeWantToReplace] != 0) {
                cashHandler.AddCash(tileToCost[tileWeWantToReplace]);
            }
        }

        if (!cashHandler.RemoveCashNoDebt(cost)) { return; }

        placeTileMap.SetTile(properlyMousTWrld, tileToPlace);
        //Implement cost thing-a-majig

        //re-generate paths for cars
        foreach(GameObject myObj in GameObject.FindGameObjectsWithTag("Car"))
        {
            myObj.GetComponent<handlePath>().GeneratePath();
        }
        //re-generate paths for buses
        foreach (GameObject myObj in GameObject.FindGameObjectsWithTag("Bus"))
        {
            myObj.GetComponent<handlePath>().GeneratePath();
        }
    }

    void RemoveNonProtectedTiles() {
        TileBase tileWeWantToReplace = placeTileMap.GetTile(properlyMousTWrld);

        foreach (TileBase protectedTile in protectedTiles)
        {
            if (protectedTile == tileWeWantToReplace)
            {
                return;
            }
        }

        if (tileWeWantToReplace != null && tileToCost[tileWeWantToReplace] != 0)
        {
            cashHandler.AddCash(tileToCost[tileWeWantToReplace]);
        }

        placeTileMap.SetTile(properlyMousTWrld, null);
    }

    void ResetCanvas()
    {
        //restore cash
        cashHandler.resetCash();
        //delete all cars
        foreach (GameObject myObj in GameObject.FindGameObjectsWithTag("Car"))
        {
            Destroy(myObj);
        }
        //delete all buses
        foreach (GameObject myObj in GameObject.FindGameObjectsWithTag("Bus"))
        {
            Destroy(myObj);
        }

        //remove start points
        GameObject.Find("Tilemap").GetComponent<handleStarts>().resetStarts();
        //reset all non protected tiles

        int x, y, z;
        for (x = placeTileMap.cellBounds.min.x; x < placeTileMap.cellBounds.max.x; x++)
        {
            for (y = placeTileMap.cellBounds.min.y; y < placeTileMap.cellBounds.max.y; y++)
            {
                for (z = placeTileMap.cellBounds.min.z; z < placeTileMap.cellBounds.max.z; z++)
                {
                    bool isProtected = false;
                    TileBase T = placeTileMap.GetTile<TileBase>(new Vector3Int(x, y, z));
                    foreach (TileBase protectedTile in protectedTiles)
                    {
                        if (protectedTile == T)
                        {
                            isProtected = true;
                        }
                    }
                    if (!isProtected) //reset non protected tiles
                    {
                        placeTileMap.SetTile(new Vector3Int(x, y, z), null);
                    }
                    //reset top tilemap
                    topTileMap.SetTile(new Vector3Int(x, y, z), null);

                }
            }

        }
    }


    // Update is called once per frame
    void Update() {
        mousTWrld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        properlyMousTWrld = placeTileMap.WorldToCell(mousTWrld);

        if (Input.GetKeyDown(KeyCode.R)) {
            currentTileInt = (currentTileInt + 1) % currentTileIntLength;
            tileToPlace = currentTiles[currentTileInt];
            currentSpriteRenderer.sprite = currentTiles[currentTileInt].sprite;

        }

        if (Input.GetButton("Fire1") && !EventSystem.current.IsPointerOverGameObject()) {
            SetNonProtectedTiles();
        }

        if (Input.GetButton("Fire2") && !EventSystem.current.IsPointerOverGameObject()) {
            RemoveNonProtectedTiles();
        }

        transform.position = mousTOffset + properlyMousTWrld;
    }
}
