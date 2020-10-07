using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Perdita
{
    public class GameController : MonoBehaviour
    {
        public static GameController instance;
        public MazeGenerator maze;
        public UIHandler uihandler;

        public GameObject player;
        public PlayerController playerController;
        public AIController AI;
        public GameObject enemyObject, enemyPrefab;
        
        public GameObject mapCamera;

        bool hasGameStarted, hasGameEnded;
        public bool isPaused;

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

        float gameTimer;

        public AudioSource musicSource, sfxSource;
        public float volume;

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

            gameTimer = 0;

            hasGameStarted = true;
            hasGameEnded = false;

            volume = PlayerPrefs.GetFloat("volume", 1);

            sfxSource.volume = musicSource.volume = volume;

            StartCoroutine(WaitBeforeEnemySpawn(3));
        }

        IEnumerator WaitBeforeEnemySpawn(float seconds)
        {
            while (seconds > 0)
            {
                seconds--;
                yield return new WaitForSeconds(1);
            }
            SpawnEnemy();
        }

        void SpawnEnemy()
        {
            enemyObject = Instantiate(enemyPrefab);
            enemyObject.transform.position = new Vector3(maze.cells[0, 0].posX, 1.0f, maze.cells[0, 0].posY);
            AI = enemyObject.GetComponent<AIController>();
            AI.player = player;
        }
        public void Initialize(Vector3 startPoint)
        {
            player.transform.position = new Vector3(maze.cells[0, 0].posX, 100, maze.cells[0, 0].posY);
            Debug.Log("Setting player position to " + player.transform.position);
            mapCamera.transform.position = new Vector3(maze.cells[size / 2, size / 2].posX, 100, maze.cells[size / 2, size / 2].posY);
            Debug.Log("Setting camera position to " + mapCamera.transform.position);

            mapCamera.GetComponent<Camera>().orthographicSize = size * 2 + 5;

            GameObject endPoint = Instantiate(endPointPrefab);
            endPoint.transform.position = new Vector3(maze.cells[size - 1, size - 1].posX, 1.5f, maze.cells[size - 1, size - 1].posY);
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
        //public GameObject rock;
        public void ThrowRock()
        {
            GameObject rock = Instantiate(rockPrefab);
            rock.SetActive(true);
            rock.transform.position = player.transform.position;
            rock.transform.rotation = player.transform.rotation;
            rock.GetComponent<Rigidbody>().AddForce(rock.transform.forward * 700f);
        }

        // Update is called once per frame
        void Update()
        {
            if (!hasGameStarted || hasGameEnded) return;

            gameTimer += Time.deltaTime;
            uihandler.UpdateTimer(gameTimer);

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

            if (!isPaused)
            {
                if (Input.GetKeyDown(KeyCode.P))
                {
                    isPaused = true;
                    uihandler.Pause(isPaused);
                    Time.timeScale = 0;
                }
            }
            else
            {
                if (Input.anyKeyDown)
                {
                    isPaused = false;
                    uihandler.Pause(isPaused);
                    Time.timeScale = 1;
                }
            }
        }

        public void PlayerCollidedWithFloor(Vector3 pos)
        {
            if (AI != null)
                AI.destination = pos;
            Debug.Log(pos);
        }

        public void SetDistraction(Vector3 pos)
        {
            distraction.GetComponent<DistractionScript>().Activate(pos);
            if(AI != null)
                AI.Chase(pos);
        }

        public void EndGame()
        {
            if (hasGameEnded) return;
            int mazeSize = PlayerPrefs.GetInt("MazeSize", 6);
            PlayerPrefs.SetInt("MazeSize", mazeSize++);
            hasGameEnded = true;
            Debug.Log("Player reached the end");
            SceneManager.LoadScene("Menu");
        }

        public void AttackPlayer()
        {
            Debug.Log("Attack player");
            playerController.DoDamage(5);
            if (playerController.health <= 0) return;
            uihandler.RedFlash();
            uihandler.UpdateHealth(playerController.health / 100f);
        }
    }
}
