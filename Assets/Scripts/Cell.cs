using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool hasBeenVisited, hasNorthWall, hasSouthWall, hasEastWall, hasWestWall;

    [SerializeField]
    float posX, posY;

    public GameObject floorPrefab, wallPrefab, wallLitPrefab;

    // Start is called before the first frame update
    void Awake()
    {
        hasBeenVisited = false;
        hasNorthWall = hasSouthWall = hasEastWall = hasWestWall = true;
        posX = posY = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPosition(float x, float y)
    {
        this.posX = x;
        this.posY = y;
    }

    public void CreateWalls()
    {
        Transform northWall = transform.Find("North Wall");
        Transform southWall = transform.Find("South Wall");
        Transform eastWall = transform.Find("East Wall");
        Transform westWall = transform.Find("West Wall");

        GameObject floor = Instantiate(floorPrefab, this.transform);
        floor.transform.position = new Vector3(posX, 0, posY);

        if (hasNorthWall && northWall == null)
        {
            GameObject obj;
            
            if (Random.value < 0.9f)
                obj = Instantiate(wallPrefab, this.transform);
            else
                obj = Instantiate(wallLitPrefab, this.transform);

            obj.transform.position = new Vector3(posX, 1.5f, posY + 2.5f);
            obj.name = "North Wall";
            obj.transform.localEulerAngles = new Vector3(0, 90, 0);
        }

        if(hasSouthWall && southWall == null)
        {
            GameObject obj;

            if (Random.value < 0.9f)
                obj = Instantiate(wallPrefab, this.transform);
            else
                obj = Instantiate(wallLitPrefab, this.transform);

            obj.transform.position = new Vector3(posX, 1.5f, posY - 2.5f);
            obj.transform.localEulerAngles = new Vector3(0, -90, 0);
            obj.name = "South Wall";
        }

        if (hasEastWall && eastWall == null)
        {
            GameObject obj;

            if (Random.value < 0.9f)
                obj = Instantiate(wallPrefab, this.transform);
            else
                obj = Instantiate(wallLitPrefab, this.transform);

            obj.transform.position = new Vector3(posX + 2.5f, 1.5f, posY);
            obj.transform.localEulerAngles = new Vector3(0, 180, 0);
            obj.name = "East Wall";
        }

        if (hasWestWall && westWall == null)
        {
            GameObject obj;

            if (Random.value < 0.9f)
                obj = Instantiate(wallPrefab, this.transform);
            else
                obj = Instantiate(wallLitPrefab, this.transform);

            obj.transform.position = new Vector3(posX - 2.5f, 1.5f, posY);
            obj.transform.localEulerAngles = new Vector3(0, 0, 0);
            obj.name = "West Wall";
        }
    }

    public void DestroyWalls()
    {
        Transform northWall = transform.Find("North Wall");
        Transform southWall = transform.Find("South Wall");
        Transform eastWall = transform.Find("East Wall");
        Transform westWall = transform.Find("West Wall");

        if (northWall != null && !hasNorthWall)
            Destroy(northWall.gameObject);
        if (southWall != null && !hasSouthWall)
            Destroy(southWall.gameObject);
        if (westWall != null && !hasWestWall)
            Destroy(westWall.gameObject);
        if (eastWall != null && !hasEastWall)
            Destroy(eastWall.gameObject);
    }
}
