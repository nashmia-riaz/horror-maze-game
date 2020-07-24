using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perdita
{
    public class GameController : MonoBehaviour
    {
        public static GameController instance;
        public MazeGenerator maze;
        public UIHandler uihandler;

        public GameObject player;
        public AIController AI;
        public GameObject mapCamera;

        public GameObject flashLight;
        public Animator flashLightAnimator;

        int size = 5;

        public float batteryPower = 100;
        public float batteryDecreaseSpeed = 1;
        float batteryFull = 100;

        bool isBatteryOn;

        SoundEffectsManager soundEffectsSource;

        public GameObject rockPrefab;

        public GameObject distraction;

        public GameObject endPointPrefab;

        // Start is called before the first frame update
        void Start()
        {
            isBatteryOn = true;

            if (instance == null)
                instance = this;
            else
                Destroy(this.gameObject);

            soundEffectsSource = GameObject.FindGameObjectWithTag("Sound Effects Source").GetComponent<SoundEffectsManager>();

            distraction = GameObject.FindGameObjectWithTag("Distraction");
        }

        public void Initialize(Vector3 startPoint)
        {
            player.transform.position = new Vector3(maze.cells[0, 0].posX, 100, maze.cells[0, 0].posY);
            Debug.Log("Setting player position to " + player.transform.position);
            mapCamera.transform.position = new Vector3(maze.cells[size / 2, size / 2].posX, 100, maze.cells[size / 2, size / 2].posY);
            Debug.Log("Setting camera position to " + mapCamera.transform.position);

            mapCamera.GetComponent<Camera>().orthographicSize = size * 2 + 5;

            GameObject endPoint = Instantiate(endPointPrefab);
            endPoint.transform.position = new Vector3(maze.cells[size-1, size-1].posX, 1.5f, maze.cells[size-1, size-1].posY);
        }

        public void ChargeBattery(float charge)
        {
            batteryPower += charge;

            if (batteryPower > batteryFull)
                batteryPower = batteryFull;

            uihandler.UpdateBattery(batteryPower / batteryFull);
            flashLightAnimator.SetFloat("BatteryPower", batteryPower);
            soundEffectsSource.PlaySound("Flashlight Rev");

            //AI.ChangeState(AIController.AIState.Chase, 4f);
            //AI.destination = player.transform.position;
            AI.Chase(player.transform.position);

            SetDistraction(player.transform.position);
        }

        public void ToggleBattery()
        {
            if (batteryPower <= 0) return;

            isBatteryOn = !isBatteryOn;
            flashLight.SetActive(isBatteryOn);
            soundEffectsSource.PlaySound("Flashlight Click");
        }

        public void ThrowRock()
        {
            GameObject rock = Instantiate(rockPrefab);
            rock.transform.position = player.transform.position;
            rock.transform.rotation = player.transform.rotation;
            rock.GetComponent<Rigidbody>().AddForce(rock.transform.forward * 700f);
        }

        // Update is called once per frame
        void Update()
        {
            if (isBatteryOn)
            {
                batteryPower -= Time.deltaTime * batteryDecreaseSpeed;
                if (batteryPower < 0)
                {
                    batteryPower = 0;
                    isBatteryOn = false;
                    flashLight.SetActive(isBatteryOn);
                }
                flashLightAnimator.SetFloat("BatteryPower", batteryPower);
                uihandler.UpdateBattery(batteryPower / batteryFull);
            }

            AI.destination = player.transform.position;
        }

        public void PlayerCollidedWithFloor(Vector3 pos)
        {
            AI.MoveTo(pos);
        }

        public void SetDistraction(Vector3 pos)
        {
            distraction.GetComponent<DistractionScript>().Activate(pos);
            AI.Chase(pos);
        }

        public void EndGame()
        {
            Debug.Log("Player reached the end");
        }
    }
}