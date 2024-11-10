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
    Tilemap placeTileMap;
    [SerializeField]
    Button BlankButton, CornerLeftButton, CornerRightButton, LaneButton, LightsButton;
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
        currentSpriteRenderer.sprite = curTile.sprite;
        cost = placeTile.TileCost;


        //Should have just used an event manager :skull:
        currentTiles = placeTile.Tiles;
        currentTileInt = placeTile.CurrentTileInt;
        currentTileIntLength = placeTile.CurrentTileIntLength;
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