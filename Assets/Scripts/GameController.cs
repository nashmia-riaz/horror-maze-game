using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the game, player and AI during the game scene.
/// </summary>
namespace Perdita
{
    public class GameController : MonoBehaviour
    {
        #region variables
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

        public GameObject endPointPrefab;

        float gameTimer;

        public AudioSource musicSource, sfxSource;
        public float volume;

        int mazeSize;
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            mazeSize = PlayerPrefs.GetInt("MazeSize", 5);
            maze.mazeSize = mazeSize;
            maze.InitializeMaze();

            mapCamera.GetComponent<Camera>().orthographicSize = mazeSize * 3;
            Vector2 bottomLeft = new Vector2(maze.cells[0, 0].posX - maze.tileSize / 2, maze.cells[0, 0].posY - maze.tileSize / 2);
            Vector2 topRight = new Vector2(maze.cells[mazeSize - 1, mazeSize-1].posX + maze.tileSize / 2, maze.cells[mazeSize - 1,mazeSize - 1].posY + maze.tileSize / 2);
            Vector2 centerMaze = (topRight + bottomLeft) / 2;
            mapCamera.transform.position = new Vector3(centerMaze.x, 100, centerMaze.y);

            isBatteryOn = true;

            if (instance == null)
                instance = this;
            else
                Destroy(this.gameObject);

            soundEffectsSource = GameObject.FindGameObjectWithTag("Sound Effects Source").GetComponent<SoundEffectsManager>();

            gameTimer = 0;

            hasGameStarted = true;
            hasGameEnded = false;

            volume = PlayerPrefs.GetFloat("volume", 1);

            sfxSource.volume = musicSource.volume = volume;

            //spawn enemy after 3 seconds
            StartCoroutine(WaitBeforeEnemySpawn(3));
        }

        /// <summary>
        /// coroutine to wait at the start of the scene before spawning the enemy
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        IEnumerator WaitBeforeEnemySpawn(float seconds)
        {
            while (seconds > 0)
            {
                seconds--;
                yield return new WaitForSeconds(1);
            }
            SpawnEnemy();
        }

        /// <summary>
        /// Spawn the enemy object
        /// </summary>
        void SpawnEnemy()
        {
            enemyObject = Instantiate(enemyPrefab);
            enemyObject.transform.position = new Vector3(maze.cells[maze.mazeSize/2, maze.mazeSize/2].posX, 1.0f, maze.cells[maze.mazeSize / 2, maze.mazeSize/2].posY);
            AI = enemyObject.GetComponent<AIController>();
            AI.player = player;
        }

        /// <summary>
        /// Initialises the game scene
        /// </summary>
        /// <param name="startPoint"></param>
        public void Initialize(Vector3 startPoint)
        {
            player.transform.position = new Vector3(maze.cells[0, 0].posX, 100, maze.cells[0, 0].posY);
            Debug.Log("Setting player position to " + player.transform.position);
            mapCamera.transform.position = new Vector3(maze.cells[size / 2, size / 2].posX, 100, maze.cells[size / 2, size / 2].posY);
            Debug.Log("Setting camera position to " + mapCamera.transform.position);

            mapCamera.GetComponent<Camera>().orthographicSize = size * 2 + 5;
        }

        /// <summary>
        /// Recharges the battery. Plays the sound.
        /// </summary>
        /// <param name="charge"></param>
        public void ChargeBattery(float charge)
        {
            batteryPower += charge;

            if (batteryPower > batteryFull)
                batteryPower = batteryFull;

            uihandler.UpdateBattery(batteryPower / batteryFull);
            flashLightAnimator.SetFloat("BatteryPower", batteryPower);
            soundEffectsSource.PlaySound("Flashlight Rev");

            AI.Chase(player);

            //SetDistraction(player.transform.position);
        }

        /// <summary>
        /// Toggles battery on or off. Plays the sound.
        /// </summary>
        public void ToggleBattery()
        {
            if (batteryPower <= 0) return;

            isBatteryOn = !isBatteryOn;
            flashLight.SetActive(isBatteryOn);
            soundEffectsSource.PlaySound("Flashlight Click");
        }

        /// <summary>
        /// Instantiates and throws a rock
        /// </summary>
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
            //if the game has not started, return and don't do anything
            if (!hasGameStarted) return;

            //if the game ended and a key is pressed, go to menu
            if (hasGameEnded)
            {
                if (Input.anyKeyDown)
                {
                    SceneManager.LoadScene("Menu");
                }
                return;
            }

            //increment game timer
            gameTimer += Time.deltaTime;
            uihandler.UpdateTimer(gameTimer);

            //deplete the battery for flashlight as time goes on
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

            //if game is paused, press any key to unpause. 
            //if game is not paused, press p to pause
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
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    SceneManager.LoadScene("Menu");
                }
                else if (Input.anyKeyDown)
                {
                    isPaused = false;
                    uihandler.Pause(isPaused);
                    Time.timeScale = 1;
                }
            }
        }

        /// <summary>
        /// called when player collides with floor to send destination on navmesh to AI
        /// </summary>
        /// <param name="pos"></param>
        public void PlayerCollidedWithFloor(Vector3 pos)
        {
            if (AI != null)
                AI.destination = pos;
        }

        /// <summary>
        /// Every time a rock is thrown, the single distraction object in game is activated and set to that position. 
        /// Distraction is a singleton. The AI can only be distracted by one distraction in the game
        /// </summary>
        /// <param name="pos"></param>
        public void SetDistraction(Vector3 pos)
        {
            DistractionScript.instance.Activate(pos);
            if(AI != null)
                AI.Chase(DistractionScript.instance.gameObject);
        }

        /// <summary>
        /// Called when the game ends i.e. when player wins. shows the end screen.
        /// </summary>
        public void EndGame()
        {
            if (hasGameEnded) return;
            int mazeSize = PlayerPrefs.GetInt("MazeSize", 5);
            PlayerPrefs.SetInt("MazeSize", mazeSize++);
            hasGameEnded = true;
            uihandler.Win();
            Time.timeScale = 0;
        }

        /// <summary>
        /// Called when game is over i.e. when player dies. Shows game over screen.
        /// </summary>
        public void GameOver()
        {
            hasGameEnded = true;
            uihandler.GameOver();
            Time.timeScale = 0;
        }

        /// <summary>
        /// Called when AI attacks player. Does damage to player
        /// </summary>
        public void AttackPlayer()
        {
            playerController.DoDamage(5);
            if (playerController.health <= 0) return;
            uihandler.RedFlash();
            uihandler.UpdateHealth(playerController.health / 100f);
        }
    }
}
