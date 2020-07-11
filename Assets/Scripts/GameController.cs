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

        public void ChargeBattery(float charge)
        {
            batteryPower += charge;

            if (batteryPower > batteryFull)
                batteryPower = batteryFull;

            uihandler.UpdateBattery(batteryPower / batteryFull);
            flashLightAnimator.SetFloat("BatteryPower", batteryPower);
            soundEffectsSource.PlaySound("Flashlight Rev");
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
        }

        public void PlayerCollidedWithFloor(Vector3 pos)
        {
            AI.MoveTo(pos);
        }

    }
}