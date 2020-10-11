using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

namespace Perdita
{
    public class AIAgent5 : Agent
    {
        public MazeGenerator mazeGenerator;
        Rigidbody m_AgentRb;
        public GameObject target;
        Vector3 startingPosition;

        // Start is called before the first frame update
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
            m_AgentRb.angularVelocity = Vector3.zero;
            transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));

            transform.position = new Vector3(mazeGenerator.cells[0, 0].posX, 1.5f, mazeGenerator.cells[0, 0].posY);
            Vector2 chosenCell = new Vector2(Random.Range(0, 2), Random.Range(0, 2));
            while(chosenCell == new Vector2(0, 0))
            {
                chosenCell = new Vector2(Random.Range(0, 2), Random.Range(0, 2));
            }
            target.transform.position = new Vector3(mazeGenerator.cells[(int) chosenCell.x, (int) chosenCell.y].posX, 0.0f, mazeGenerator.cells[(int)chosenCell.x, (int)chosenCell.y].posY);
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(transform.InverseTransformDirection(m_AgentRb.velocity));
            sensor.AddObservation(target.transform.localPosition);
            sensor.AddObservation(this.transform.localPosition);
            //sensor.AddObservation(m_AgentRb.velocity.x);
            //sensor.AddObservation(m_AgentRb.velocity.z);
        }

        public override void OnActionReceived(ActionBuffers actionBuffers)
        {
            AddReward(-0.001f);
            MoveAgent(actionBuffers.DiscreteActions);
        }

        //public float forceMultiplier = 20;
        //public override void OnActionReceived(float[] vectorAction)
        //{
        //    Vector3 controlSignal = Vector3.zero;
        //    controlSignal.x = vectorAction[0];
        //    controlSignal.z = vectorAction[1];
        //    m_AgentRb.AddForce(controlSignal * forceMultiplier);
        //}

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
                Debug.Log("Restarting episode: AI found player");
                EndEpisode();
            }
            else if (collision.gameObject.CompareTag("Wall"))
            {
                AddReward(-0.01f);
                Debug.Log("Restarting episode: AI ran into a wall");
                //EndEpisode();
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

        //public override void Heuristic(float[] actionsOut)
        //{
        //    actionsOut[0] = Input.GetAxis("Horizontal");
        //    actionsOut[1] = Input.GetAxis("Vertical");
        //}
    }
}