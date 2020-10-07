using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Perdita
{
    public class PlayerAI : MonoBehaviour
    {
        NavMeshAgent agent;
        public MazeGenerator maze;

        IEnumerator WaitPatrol(float seconds)
        {
            while (seconds > 0)
            {
                seconds--;
                yield return new WaitForSeconds(1);
            }
            GoToPoint();
            StartCoroutine(WaitPatrol(5));
        }

        void GoToPoint()
        {
            Vector2 cellToGoTo = new Vector2(Random.Range(0, maze.mazeSize), Random.Range(0, maze.mazeSize));
            Vector3 destination = new Vector3(maze.cells[(int)cellToGoTo.x, (int)cellToGoTo.y].posX, 0, maze.cells[(int)cellToGoTo.x, (int)cellToGoTo.y].posY);
            agent.SetDestination(destination);
        }
        // Start is called before the first frame update
        void Start()
        {
            agent = gameObject.GetComponent<NavMeshAgent>();
            StartCoroutine(WaitPatrol(5));
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}