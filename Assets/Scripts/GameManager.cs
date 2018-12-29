using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour 
{
    Camera mainCamera;

    public GameObject tile;

    Tile[] playerHand = new Tile[3];
    Vector3[] handPositions = new Vector3[3];
    List<Tile> deck = new List<Tile>();
    GridManager gridMan;

	// Use this for initialization
	void Awake () {
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        gridMan = this.GetComponent<GridManager>();
        handPositions[0] = new Vector3(-1.5f, 0, -3.0f);
        handPositions[1] = new Vector3(0, 0, -3.0f);
        handPositions[2] = new Vector3(1.5f, 0, -3.0f);
	}

    void Start()
    {
        gridMan.CreateGrid();
        ResetGame();
    }

    public void ResetGame()
    {
        ClearGame();
        CreateDeck();
        PlaceRandomTile();
        DealTile();
        DealTile();
        DealTile();
    }

    void ClearGame()
    {
        gridMan.ClearPlacements();
        for (int a = 0; a < playerHand.Length; a++)
        {
            if (playerHand[a] != null)
            {
                Destroy(playerHand[a].gameObject);
            }
            playerHand[a] = null;
        }
        int deckStart = deck.Count;
        for (int a = 0; a < deckStart; a++)
        {
            GameObject remove = deck[0].gameObject;
            deck.RemoveAt(0);
            Destroy(remove);
        }
    }

    void CreateDeck()
    {
        List<GameObject> newTiles = new List<GameObject>();
        for (int a = 0; a < 10; a++)
        {
            GameObject newTile = GameObject.Instantiate(tile, this.transform) as GameObject;
            newTile.GetComponent<Tile>().SetTileState(Tile.TileState.InDeck);
            newTile.GetComponent<Tile>().tileValue = a + 1;
            newTiles.Add(newTile);
        }
        while (newTiles.Count > 0)
        {
            int shuffleIndex = Random.Range(0, newTiles.Count);
            deck.Add(newTiles[shuffleIndex].GetComponent<Tile>());
            newTiles.RemoveAt(shuffleIndex);
        }
    }

    void PlaceRandomTile()
    {
        if (deck.Count <= 0)
            return;
        int randX = Random.Range(0, gridMan.gridWidth);
        int randY = Random.Range(0, gridMan.gridHeight);
        Tile newTile = deck[0];
        gridMan.PlaceTile(newTile, randX, randY);
        deck.RemoveAt(0);
    }

    void DealTile()
    {
        if (deck.Count <= 0)
            return;
        int placeIndex = playerHand.Length;
        for (int a = 0; a < playerHand.Length; a++)
        {
            if (playerHand[a] == null)
            {
                placeIndex = a;
                break;
            }
        }
        if (placeIndex >= playerHand.Length)
            return;
        Tile newTile = deck[0];
        playerHand[placeIndex] = newTile;
        deck.RemoveAt(0);
        newTile.transform.position = handPositions[placeIndex];
        newTile.SetTileState(Tile.TileState.InHand);
    }
	
	// Update is called once per frame
    int activeTileIndex = 3;
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity))
            {
                activeTileIndex = 3;
                for (int a = 0; a < playerHand.Length; a++)
                {
                    if (playerHand[a] != null && hit.collider.gameObject == playerHand[a].gameObject)
                    {
                        playerHand[a].SetTileState(Tile.TileState.InPlacement);
                        activeTileIndex = a;
                        gridMan.MarkAvailability();
                        break;
                    }
                }
            }
        }
        else if (Input.GetMouseButton(0))
        {
            if (activeTileIndex < 3)
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                float camHeight = mainCamera.transform.position.y;
                float vectMult = -camHeight / ray.direction.y;
                Vector3 movePosition = new Vector3(ray.origin.x + (vectMult * ray.direction.x), 0, ray.origin.z + (vectMult * ray.direction.z));
                playerHand[activeTileIndex].transform.position = movePosition;
                gridMan.UpdateAvailability(movePosition);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (activeTileIndex < 3)
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                float camHeight = mainCamera.transform.position.y;
                float vectMult = -camHeight / ray.direction.y;
                Vector3 movePosition = new Vector3(ray.origin.x + (vectMult * ray.direction.x), 0, ray.origin.z + (vectMult * ray.direction.z));
                if (gridMan.PlaceTile(playerHand[activeTileIndex], movePosition))
                {
                    playerHand[activeTileIndex] = null;
                    DealTile();
                    activeTileIndex = 3;
                }
                else
                {
                    playerHand[activeTileIndex].transform.position = handPositions[activeTileIndex];
                    activeTileIndex = 3;
                }
            }
        }
	}
}
