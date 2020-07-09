using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Perdita
{
    public class AIController : MonoBehaviour
    {
        public Camera cam;
        public NavMeshAgent agent;

        bool isCollidingWithPlayer = false;

        // Start is called before the first frame update
        void Start()
        {
            agent = gameObject.GetComponent<NavMeshAgent>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    agent.SetDestination(hit.point);
                }
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
            Debug.Log("Moving AI to " + point);
            agent.SetDestination(point);
        }
    }
}
