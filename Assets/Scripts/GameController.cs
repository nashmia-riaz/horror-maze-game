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

        // Start is called before the first frame update
        void Start()
        {
            isBatteryOn = true;

            if (instance == null)
                instance = this;
            else
                Destroy(this.gameObject);

            soundEffectsSource = GameObject.FindGameObjectWithTag("Sound Effects Source").GetComponent<SoundEffectsManager>();
        }

        public void Initialize(Vector3 startPoint)
        {
            player.transform.position = new Vector3(maze.cells[0, 0].posX, 100, maze.cells[0, 0].posY);
            Debug.Log("Setting player position to " + player.transform.position);
            mapCamera.transform.position = new Vector3(maze.cells[size / 2, size / 2].posX, 100, maze.cells[size / 2, size / 2].posY);
            Debug.Log("Setting camera position to " + mapCamera.transform.position);

            mapCamera.GetComponent<Camera>().orthographicSize = size * 2 + 5;
        }

        void ChargeBattery(float charge)
        {
            batteryPower += charge;

            if (batteryPower > batteryFull)
                batteryPower = batteryFull;

            uihandler.UpdateBattery(batteryPower / batteryFull);
            flashLightAnimator.SetFloat("BatterPower", batteryPower);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F) && batteryPower > 0)
            {
                isBatteryOn = !isBatteryOn;
                flashLight.SetActive(isBatteryOn);
                soundEffectsSource.PlaySound("Flashlight Click");
            }
            else if (Input.GetMouseButtonDown(1))
            {
                ChargeBattery(10);
            }

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
        }

        public void PlayerCollidedWithFloor(Vector3 pos)
        {
            Debug.Log("Moving AI to " + pos);
            AI.MoveTo(pos);
        }

    }
}