using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public MazeGenerator maze;

    public GameObject player;
    public GameObject AI;
    public GameObject mapCamera;

    int size = 5;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Initialize(Vector3 startPoint)
    {
        player.transform.position = new Vector3(maze.cells[0, 0].posX, 100, maze.cells[0, 0].posY); 
        Debug.Log("Setting player position to " + player.transform.position);
        mapCamera.transform.position = new Vector3(maze.cells[size/2, size/2].posX, 100, maze.cells[size/2, size/2].posY);
        Debug.Log("Setting camera position to " + mapCamera.transform.position);

        mapCamera.GetComponent<Camera>().orthographicSize = size * 2 + 5;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
