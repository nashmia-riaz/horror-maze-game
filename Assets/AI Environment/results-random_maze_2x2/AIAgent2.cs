using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

namespace Perdita
{
    public class AIAgent2 : Agent
    {
        public MazeGenerator mazeGenerator;
        Rigidbody m_AgentRb;
        public GameObject target;
        Vector3 startingPosition;

        void Start()
        {
            m_AgentRb = GetComponent<Rigidbody>();
            startingPosition = transform.position;
        }

        public override void OnEpisodeBegin()
        {
            mazeGenerator.ResetMaze();
            transform.position = startingPosition;
            m_AgentRb.velocity = Vector3.zero;
            transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));

            transform.position = new Vector3(mazeGenerator.cells[0, 0].posX, 1.5f, mazeGenerator.cells[0, 0].posY);
            target.transform.position = new Vector3(mazeGenerator.cells[1, 1].posX, 1.1f, mazeGenerator.cells[1, 1].posY);
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(transform.InverseTransformDirection(m_AgentRb.velocity));
            sensor.AddObservation(target.transform.localPosition);
            sensor.AddObservation(this.transform.localPosition);
        }
        public override void OnActionReceived(ActionBuffers actionBuffers)
        {
            AddReward(-0.001f);
            MoveAgent(actionBuffers.DiscreteActions);
        }

        public void MoveAgent(ActionSegment<int> act)
        {
            var dirToGo = Vector3.zero;
            var rotateDir = Vector3.zero;

            var action = act[0];
            switch (action)
            {
                case 1:
                    dirToGo = transform.forward * 1f;
                    break;
                case 2:
                    dirToGo = transform.forward * -1f;
                    break;
                case 3:
                    rotateDir = transform.up * 1f;
                    break;
                case 4:
                    rotateDir = transform.up * -1f;
                    break;
            }
            transform.Rotate(rotateDir, Time.deltaTime * 200f);
            m_AgentRb.AddForce(dirToGo * 0.5f, ForceMode.VelocityChange);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                SetReward(2f);
                EndEpisode();
            }
            else if (collision.gameObject.CompareTag("Wall"))
            {
                SetReward(-0.5f);
            }
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var discreteActionsOut = actionsOut.DiscreteActions;
            discreteActionsOut[0] = 0;
            if (Input.GetKey(KeyCode.D))
            {
                Debug.Log("Pressed D");
                discreteActionsOut[0] = 3;
            }
            else if (Input.GetKey(KeyCode.W))
            {
                Debug.Log("Pressed D");
                discreteActionsOut[0] = 1;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                Debug.Log("Pressed D");
                discreteActionsOut[0] = 4;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                Debug.Log("Pressed D");
                discreteActionsOut[0] = 2;
            }
        }
    }
}