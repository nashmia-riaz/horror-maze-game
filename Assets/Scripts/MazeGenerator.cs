using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Generates a maze using Hunt-and-Kill algorithm.
/// </summary>
namespace Perdita
{
    public class MazeGenerator : MonoBehaviour
    {
        #region variables
        public NavMeshSurface navMesh;
        int rowCount, colCount;

        public Cell[,] cells;
        GameObject[,] cellObjects;
        public GameObject cellPrefab;

        int currentRow, currentCol;

        bool courseComplete;

        public int tileSize = 5;
        public int mazeSize;

        public GameController gameController;

        public Transform mazeObject;
        public GameObject EndPoint;

        bool isMazeInitialized;
        public bool shouldInitializeMaze;
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            if (!shouldInitializeMaze) return;

            isMazeInitialized = false;
            InitializeMaze();
        }

        /// <summary>
        /// Destroys the maze currently generated.
        /// </summary>
        void DestroyMaze()
        {
            if (!isMazeInitialized) return;

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    Destroy(cellObjects[i, j]);
                }
            }

            isMazeInitialized = false;
        }

        /// <summary>
        /// Destroys and creates a new maze. Used mainly in AI training for each episode.
        /// </summary>
        public void ResetMaze()
        {
            DestroyMaze();
            InitializeMaze();
        }
        

        /// <summary>
        /// Initialises the maze as a grid with all walls present.
        /// Then applies Hunt-and-Kill to carve and create paths.
        /// </summary>
        public void InitializeMaze()
        {
            if (mazeSize == 0)
                mazeSize = PlayerPrefs.GetInt("MazeSize", 6);
            courseComplete = false;
            currentCol = currentRow = 0;
            rowCount = colCount = mazeSize;

            cellObjects = new GameObject[rowCount, colCount];
            cells = new Cell[rowCount, colCount];

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    cellObjects[i, j] = Instantiate(cellPrefab);
                    cells[i, j] = cellObjects[i, j].GetComponent<Cell>();
                    cellObjects[i, j].name = "Cell (" + i + ", " + j + ")";
                    cellObjects[i, j].transform.SetParent(mazeObject, true);

                    cells[i, j].SetPosition(i * tileSize, j * tileSize);
                    cells[i, j].hasWestWall = false;
                    cells[i, j].hasNorthWall = false;

                    if (i == 0)
                        cells[i, j].hasWestWall = true;
                    if (j == colCount - 1)
                        cells[i, j].hasNorthWall = true;

                    if (i == rowCount - 1 && j == colCount - 1)
                    {
                        EndPoint.transform.position = new Vector3(i * tileSize, 1.5f, j * tileSize);
                    }
                }
                print("Making cells for " + i);
            }

            HuntAndKill();
            RenderMaze();

            //generates navmesh surfaces at runtime
            navMesh.BuildNavMesh();

            if (gameController != null)
                gameController.Initialize(cells[0, 0].transform.position + new Vector3(0, 1.5f, 0));

            isMazeInitialized = true;
        }

        /// <summary>
        /// Renders the maze and creates its walls
        /// </summary>
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

        /// <summary>
        /// applies Hunt-and-Kill until no more paths are creatable.
        /// </summary>
        public void HuntAndKill()
        {
            while (!courseComplete)
            {
                Kill();
                Hunt();
            }
        }

        #region hunt
        /// <summary>
        /// Hunts to find a cell who's neighbor was previously visited and sets it as current cell.
        /// Connects it with previously visited neighbor by destroying adjacent wall.
        /// </summary>
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
        /// <summary>
        /// Chooses a random direction to go to from current cell and visits that cell.
        /// </summary>
        public void Kill()
        {
            while (RouteStillAvailable(currentRow, currentCol))
            {
                int direction = Random.Range(1, 4);

                if (currentRow == 0 && currentCol == 0)
                    cells[0, 0].hasBeenVisited = true;

                //going up
                if (direction == 1 && isCellAvailable(currentRow, currentCol + 1))
                {
                    cells[currentRow, currentCol].hasNorthWall = false;
                    cells[currentRow, currentCol + 1].hasSouthWall = false;
                    currentCol++;
                }

                //going down
                else if (direction == 2 && isCellAvailable(currentRow, currentCol - 1))
                {
                    cells[currentRow, currentCol].hasSouthWall = false;
                    cells[currentRow, currentCol - 1].hasNorthWall = false;
                    currentCol--;
                }

                //going right
                else if (direction == 3 && isCellAvailable(currentRow + 1, currentCol))
                {
                    cells[currentRow, currentCol].hasEastWall = false;
                    cells[currentRow + 1, currentCol].hasWestWall = false;
                    currentRow++;
                }

                //going left
                else if (direction == 4 && isCellAvailable(currentRow - 1, currentCol))
                {
                    cells[currentRow, currentCol].hasWestWall = false;
                    cells[currentRow - 1, currentCol].hasEastWall = false;
                    currentRow--;
                }

                cells[currentRow, currentCol].hasBeenVisited = true;
            }
        }

        /// <summary>
        /// Checks to see if a route is available to the cell specified.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
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

            if (col < colCount - 1 && !cells[row, col + 1].hasBeenVisited)
                availableRoutes++;

            return availableRoutes > 0;
        }

        //check if cell is available to be visited
        bool isCellAvailable(int row, int col)
        {
            return (row >= 0 && row < rowCount && col >= 0 && col < colCount && !cells[row, col].hasBeenVisited);
        }

        #endregion
    }
}