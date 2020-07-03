using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public MazeGenerator maze;
    public UIHandler uihandler;

    public GameObject player;
    public GameObject AI;
    public GameObject mapCamera;

    public GameObject flashLight;
    public Animator flashLightAnimator;

    int size = 5;

    public float batteryPower = 100;
    public float batteryDecreaseSpeed = 1;
    float batteryFull = 100;

    bool isBatteryOn;

    // Start is called before the first frame update
    void Start()
    {
        isBatteryOn = true;
    }

    public void Initialize(Vector3 startPoint)
    {
        player.transform.position = new Vector3(maze.cells[0, 0].posX, 100, maze.cells[0, 0].posY); 
        Debug.Log("Setting player position to " + player.transform.position);
        mapCamera.transform.position = new Vector3(maze.cells[size/2, size/2].posX, 100, maze.cells[size/2, size/2].posY);
        Debug.Log("Setting camera position to " + mapCamera.transform.position);

        mapCamera.GetComponent<Camera>().orthographicSize = size * 2 + 5;
    }

    void ChargeBattery(float charge) {
        batteryPower += charge;

        if (batteryPower > batteryFull)
            batteryPower = batteryFull;

        uihandler.UpdateBattery(batteryPower / batteryFull);
        flashLightAnimator.SetFloat("BatterPower", batteryPower);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isBatteryOn = !isBatteryOn;
            flashLight.SetActive(isBatteryOn);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            ChargeBattery(10);
        }

        if (isBatteryOn)
        {
            batteryPower -= Time.deltaTime * batteryDecreaseSpeed;
            if (batteryPower < 0) batteryPower = 0;
            flashLightAnimator.SetFloat("BatteryPower", batteryPower);
            uihandler.UpdateBattery(batteryPower / batteryFull);
        }
    }
}
