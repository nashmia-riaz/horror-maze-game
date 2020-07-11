using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Perdita
{
    public class AIController : MonoBehaviour
    {
        public enum AIState
        {
            Patrol,
            Chase, 
            Idle
        }

        public AIState state;
        public Camera cam;
        public NavMeshAgent agent;

        public GameObject player;

        bool isCollidingWithPlayer = false;

        public float speed;

        // Start is called before the first frame update
        void Start()
        {
            speed = 5;
            player = GameObject.FindGameObjectWithTag("Player");
            agent = gameObject.GetComponent<NavMeshAgent>();
        }

        public void Chase()
        {
            speed = 4f;
            state = AIState.Chase;
            agent.speed = speed;
        }

        public void Idle()
        {
            state = AIState.Idle;
            speed = 0;
            agent.speed = speed;
        }

        public void Patrol()
        {
            speed = 2f;
            state = AIState.Patrol;
            agent.speed = speed;
        }

        // Update is called once per frame
        void Update()
        {
            if(state == AIState.Chase)
            {
                agent.SetDestination(player.transform.position);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player" && !isCollidingWithPlayer)
            {
                agent.SetDestination(transform.position);
                isCollidingWithPlayer = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player" && isCollidingWithPlayer)
                isCollidingWithPlayer = false;
        }

        public void MoveTo(Vector3 point)
        {
            if (isCollidingWithPlayer) return;
            agent.SetDestination(point);
        }
    }
}
