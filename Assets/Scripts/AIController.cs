﻿using System.Collections;
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
            Idle,
            Attack
        }

        public AIState state;
        public Camera cam;
        public NavMeshAgent agent;

        public GameObject player;

        bool isCollidingWithPlayer = false;

        public float speed;

        public Vector3 destination;

        DistractionScript distraction;

        delegate void StateChangeDelegate();
        StateChangeDelegate stateChange;

        // Start is called before the first frame update
        void Start()
        {
            speed = 5;
            player = GameObject.FindGameObjectWithTag("Player");
            agent = gameObject.GetComponent<NavMeshAgent>();
            distraction = GameObject.FindGameObjectWithTag("Distraction").GetComponent<DistractionScript>();
            Patrol();
        }

        public void ChangeState(AIState newState, float newSpeed)
        {
            state = newState;
            speed = newSpeed;
            agent.speed = speed;
        }

        public void Chase(Vector3 newDestination)
        {
            Debug.Log("AI is chasing");
            StopAllCoroutines();
            destination = newDestination;
            agent.SetDestination(destination);

            state = AIState.Chase;
            speed = 4f;
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

        public void Attack()
        {
            speed = 0;
            state = AIState.Attack;
            agent.speed = speed;
            GameController.instance.AttackPlayer();
        }

        // Update is called once per frame
        void Update()
        {
            //if AI is patrolling and has not reached target, keep going for the target
            if(state == AIState.Patrol && agent.remainingDistance != 0 && !isCollidingWithPlayer)
            {
                agent.SetDestination(GameController.instance.player.transform.position);
            }
        }

        IEnumerator WaitIdle(float time, StateChangeDelegate newState)
        {
            while(time > 0)
            {
                time--;
                yield return new WaitForSeconds(1);
            }
            newState();
        }

        void EndDistraction()
        {
            distraction.Deactivate();
            Patrol();
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player" && !isCollidingWithPlayer)
            {
                isCollidingWithPlayer = true;
                Attack();

                StartCoroutine(WaitIdle(2f, ()=> {
                    Attack();
                }));
            }

            if (other.gameObject.tag == "Distraction")
            {
                Debug.Log("Collided with distraction");

                if (other.gameObject.GetComponent<DistractionScript>().IsActive)
                {
                    Debug.Log("AI is going idle");
                    state = AIState.Idle;
                    StartCoroutine(WaitIdle(5f, EndDistraction));
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player" && isCollidingWithPlayer)
            {
                isCollidingWithPlayer = false;
                Patrol();
            }
        }

        public void MoveTo(Vector3 point)
        {
            if (isCollidingWithPlayer) return;
            if (state != AIState.Patrol) return;
            agent.SetDestination(point);
        }

        private void OnCollisionEnter(Collision collision)
        {
        }
    }
}
