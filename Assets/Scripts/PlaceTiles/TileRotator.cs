using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;

//Deals with changing the tile buttons when a player rotates it
public class TileRotator : MonoBehaviour
{
    [SerializeField]
    Tile[] tiles;
    [SerializeField]
    int tileCost;
    [SerializeField]
    TextMeshProUGUI costText;
    [SerializeField]
    Tilemap placeableTilemaps;
    int currentTileInt;
    int currentTileIntLength;

    Tile currentTile;

    Image currentSpriteRenderer;

    public Tilemap PlaceableTilemaps => placeableTilemaps;
    public Tile CurrentTile => currentTile;
    public Tile[] Tiles => tiles;
    public int TileCost => tileCost;
    public int CurrentTileInt => currentTileInt;
    public int CurrentTileIntLength => currentTileIntLength;

    // Start is called before the first frame update
    void Start()
    {
        currentSpriteRenderer = GetComponent<Image>();
        currentTileIntLength = tiles.Length;

        currentTileInt = 0;
        currentTile = tiles[currentTileInt];
        currentSpriteRenderer.sprite = currentTile.sprite;

        costText.text = "$" + tileCost;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R)) {
            currentTileInt = (currentTileInt + 1) % currentTileIntLength;
            currentTile = tiles[currentTileInt];
            currentSpriteRenderer.sprite = currentTile.sprite;

        }
    }
}
