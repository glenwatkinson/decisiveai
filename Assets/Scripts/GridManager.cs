using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour {

    public int gridWidth = 8;
    public int gridHeight = 8;

    public GameObject blackGrid;
    public GameObject whiteGrid;

    GridSpace[,] grid;
    public Tile[,] placedTiles;

	// Use this for initialization
	void Start () {
		
	}

    public void ClearPlacements()
    {
        if (placedTiles == null)
            return;
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (placedTiles[x,y] != null)
                {
                    Destroy(placedTiles[x,y].gameObject);
                    placedTiles[x, y] = null;
                }
            }
        }
    }
	
    public void CreateGrid()
    {
        grid = new GridSpace[gridWidth, gridHeight];
        placedTiles = new Tile[gridWidth, gridHeight];
        float xOffset = -(gridWidth / 2.0f) + 0.5f;
        float yOffset = -(gridHeight / 2.0f) + 0.5f;
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                GameObject newGrid;
                if ((x + y)%2 == 0)
                    newGrid = GameObject.Instantiate(blackGrid, this.transform) as GameObject;
                else
                    newGrid = GameObject.Instantiate(whiteGrid, this.transform) as GameObject;
                newGrid.transform.localPosition = new Vector3(xOffset + x, -0.1f, yOffset + y);
                grid[x,y] = newGrid.GetComponent<GridSpace>();
                grid[x, y].SetGridState(GridSpace.GridState.Inactive);
            }
        }
    }

    public void RecalculateSums()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (placedTiles[x, y] != null)
                {
                    int sumValue = placedTiles[x, y].tileValue;
                    if (x > 0 && placedTiles[x - 1, y] != null)
                        sumValue += placedTiles[x - 1, y].tileValue;
                    if (y > 0 && placedTiles[x, y - 1] != null)
                        sumValue += placedTiles[x, y - 1].tileValue;
                    if (x < gridWidth - 2 && placedTiles[x + 1, y] != null)
                        sumValue += placedTiles[x + 1, y].tileValue;
                    if (y < gridHeight - 2 && placedTiles[x, y + 1] != null)
                        sumValue += placedTiles[x, y + 1].tileValue;
                    placedTiles[x, y].sumValue = sumValue;
                    placedTiles[x, y].SetTileState(Tile.TileState.OnGrid);
                }
            }
        }
    }

    public void PlaceTile(Tile tile, int xPos, int yPos)
    {
        tile.transform.position = new Vector3(grid[xPos, yPos].transform.position.x, 0, grid[xPos, yPos].transform.position.z);
        tile.SetTileState(Tile.TileState.OnGrid);
        placedTiles[xPos, yPos] = tile;
        RecalculateSums();
    }

    public bool PlaceTile(Tile tile, Vector3 pos)
    {
        float bestDistance = 2.0f;
        int markedX = gridWidth;
        int markedY = gridHeight;
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y].currentState == GridSpace.GridState.Available || grid[x, y].currentState == GridSpace.GridState.PlacementOption)
                {
                    float checkDistance = Vector3.Distance(grid[x, y].transform.position, pos);
                    if (checkDistance < bestDistance)
                    {
                        bestDistance = checkDistance;
                        markedX = x;
                        markedY = y;
                    }
                }
                grid[x, y].SetGridState(GridSpace.GridState.Inactive);
            }
        }
        if (markedX < gridWidth)
        {
            tile.transform.position = new Vector3(grid[markedX, markedY].transform.position.x, 0, grid[markedX, markedY].transform.position.z);
            placedTiles[markedX, markedY] = tile;
            tile.SetTileState(Tile.TileState.OnGrid);
            RecalculateSums();
            return true;
        }
        return false;
    }

    public void UpdateAvailability(Vector3 pos)
    {
        float bestDistance = 2.0f;
        int markedX = gridWidth;
        int markedY = gridHeight;
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y].currentState == GridSpace.GridState.Available || grid[x, y].currentState == GridSpace.GridState.PlacementOption)
                {
                    grid[x, y].SetGridState(GridSpace.GridState.Available);
                    float checkDistance = Vector3.Distance(grid[x, y].transform.position, pos);
                    if (checkDistance < bestDistance)
                    {
                        bestDistance = checkDistance;
                        markedX = x;
                        markedY = y;
                    }
                }
            }
        }
        if (markedX < gridWidth)
            grid[markedX, markedY].SetGridState(GridSpace.GridState.PlacementOption);
    }

    public void MarkAvailability()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                bool hasAdjacent = false;
                if (placedTiles[x, y] == null)
                {
                    if (x > 0 && placedTiles[x - 1, y] != null)
                        hasAdjacent = true;
                    else if (x < gridWidth - 1 && placedTiles[x + 1, y] != null)
                        hasAdjacent = true;
                    else if (y > 0 && placedTiles[x, y - 1] != null)
                        hasAdjacent = true;
                    else if (y < gridWidth - 1 && placedTiles[x, y + 1] != null)
                        hasAdjacent = true;
                }
                if (hasAdjacent)
                    grid[x, y].SetGridState(GridSpace.GridState.Available);
                else
                    grid[x, y].SetGridState(GridSpace.GridState.Inactive);

            }
        }
    }

    public void ClearAvailability()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                grid[x, y].SetGridState(GridSpace.GridState.Inactive);
            }
        }
    }

	// Update is called once per frame
	void Update () {
		
	}
}
