using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceObjectScript : MonoBehaviour
{
    public Transform FaceObject;

    // Start is called before the first frame update
    void Start()
    {
        if(FaceObject == null)
            FaceObject = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(FaceObject);
    }
}
