using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class MazeGenerator : MonoBehaviour
{
    public NavMeshSurface navMesh;
    int rowCount, colCount;

    public Cell[,] cells;
    GameObject[,] cellObjects;
    public GameObject cellPrefab;

    int currentRow, currentCol;

    bool courseComplete;

    public int mazeSize = 6;

    public GameController gameController;

    // Start is called before the first frame update
    void Start()
    {
        courseComplete = false;
        currentCol = currentRow = 0;
        rowCount = colCount = mazeSize;

        cellObjects = new GameObject[rowCount, colCount];
        cells = new Cell[rowCount, colCount];

        for(int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < colCount; j++)
            {
                cellObjects[i, j] = Instantiate(cellPrefab);
                cells[i, j] = cellObjects[i, j].GetComponent<Cell>();
                cellObjects[i, j].name = "Cell (" + i + ", " + j + ")";

                cells[i, j].SetPosition(i * mazeSize, j * mazeSize);
                cells[i, j].hasWestWall = false;
                cells[i, j].hasNorthWall = false;

                if (i == 0)
                    cells[i, j].hasWestWall = true;
                if (j == colCount-1)
                    cells[i, j].hasNorthWall = true;
            }
            print("Making cells for " + i);
        }

        HuntAndKill();
        RenderMaze();
        navMesh.BuildNavMesh();
        gameController.Initialize(cells[0, 0].transform.position + new Vector3(0, 1.5f, 0));
    }
     
    public void RenderMaze()
    {
        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < colCount; j++)
            {
                cells[i, j].DestroyWalls();
                cells[i, j].CreateWalls();
            }
        }

    }
    public void HuntAndKill()
    {
        while (!courseComplete)
        {
            Kill();
            Hunt();
        }
    }

    #region hunt
    public void Hunt()
    {
        courseComplete = true;

        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                if (!cells[row, col].hasBeenVisited && isAdjacentCellVisited(row, col))
                {
                    courseComplete = false;
                    currentRow = row;
                    currentCol = col;
                    //when new cell is found, destroy adjacent wall with previous kill path
                    DestroyAdjacentWall(currentRow, currentCol);
                    cells[currentRow, currentCol].hasBeenVisited = true;
                    return;
                }
            }
        }
    }

   //destroy wall next to current cell, giving access to unvisited cell
    void DestroyAdjacentWall(int row, int col)
    {
        bool wallDestroyed = false;
        while (!wallDestroyed)
        {
            int direction = Random.Range(1, 4);

            //going right
            if (direction == 1 && row > 0 && cells[row - 1, col].hasBeenVisited)
            {
                cells[row, col].hasWestWall = false;
                cells[row - 1, col].hasEastWall = false;
                wallDestroyed = true;
            }
            //going right
            else if (direction == 2 && row < (rowCount - 2) && cells[row + 1, col].hasBeenVisited)
            {
                cells[row, col].hasEastWall = false;
                cells[row + 1, col].hasWestWall = false;
                wallDestroyed = true;
            }
            //going down
            else if (direction == 3 && col > 0 && cells[row, col - 1].hasBeenVisited)
            {
                cells[row, col].hasSouthWall = false;
                cells[row, col - 1].hasNorthWall = false;
                wallDestroyed = true;
            }
            //going up
            else if (direction == 4 && col < (colCount - 2) && cells[row, col + 1].hasBeenVisited)
            {
                cells[row, col].hasNorthWall = false;
                cells[row, col + 1].hasSouthWall = false;
                wallDestroyed = true;
            }
        }
    }

    //check if any adjacent cells to current cells have been visited
    bool isAdjacentCellVisited(int row, int col)
    {
        int visitedCells = 0;

        if (row > 0 && cells[row - 1, col].hasBeenVisited)
            visitedCells++;
        if (row < rowCount - 2 && cells[row + 1, col].hasBeenVisited)
            visitedCells++;
        if (col > 0 && cells[row, col - 1].hasBeenVisited)
            visitedCells++;
        if (col < colCount - 2 && cells[row, col + 1].hasBeenVisited)
            visitedCells++;

        return visitedCells > 0;
    }
    #endregion

    #region kill
    public void Kill()
    {
        while (RouteStillAvailable(currentRow, currentCol))
        {
            int direction = Random.Range(1, 4);

            if (currentRow == 0 && currentCol == 0)
                cells[0,0].hasBeenVisited = true;

            //going up
            if (direction == 1 && isCellAvailable(currentRow, currentCol+1))
            {
                cells[currentRow, currentCol].hasNorthWall = false;
                cells[currentRow, currentCol +1].hasSouthWall = false;
                currentCol++;
            }

            //going down
            else if (direction == 2 && isCellAvailable(currentRow , currentCol-1))
            {
                cells[currentRow, currentCol].hasSouthWall = false;
                cells[currentRow, currentCol -1].hasNorthWall = false;
                currentCol--;
            }

            //going right
            else if (direction == 3 && isCellAvailable(currentRow+1, currentCol))
            {
                cells[currentRow, currentCol].hasEastWall = false;
                cells[currentRow+1, currentCol].hasWestWall = false;
                currentRow++;
            }

            //going left
            else if (direction == 4 && isCellAvailable(currentRow-1, currentCol))
            {
                cells[currentRow, currentCol].hasWestWall = false;
                cells[currentRow-1, currentCol].hasEastWall = false;
                currentRow--;
            }

            cells[currentRow, currentCol].hasBeenVisited = true;
        }
    }
    
    bool RouteStillAvailable(int row, int col)
    {
        if (cells[row, col].hasBeenVisited) return false;

        int availableRoutes = 0;

        if (row > 0 && !cells[row - 1, col].hasBeenVisited)
            availableRoutes++;

        if (row < rowCount - 1 && !cells[row + 1, col].hasBeenVisited)
            availableRoutes++;

        if (col > 0 && !cells[row, col - 1].hasBeenVisited)
            availableRoutes++;

        if (col < colCount - 1 && !cells[row,col + 1].hasBeenVisited)
            availableRoutes++;

        return availableRoutes > 0;
    }

    //check if cell is available to be visited
    bool isCellAvailable(int row, int col)
    {
        return (row >= 0 && row < rowCount && col >= 0 && col < colCount && !cells[row, col].hasBeenVisited);
    }

    #endregion

    // Update is called once per frame
    void Update()
    {
        
    }
}
