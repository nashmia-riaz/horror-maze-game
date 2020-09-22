using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceObjectScript : MonoBehaviour
{
    public Transform FaceObject;
    public string faceObjectTag;

    // Start is called before the first frame update
    void Start()
    {
        if(FaceObject == null && faceObjectTag != null && faceObjectTag != "")
            FaceObject = GameObject.FindGameObjectWithTag(faceObjectTag).transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(FaceObject != null)
            transform.LookAt(FaceObject);
    }
}
