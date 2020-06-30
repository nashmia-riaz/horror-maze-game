using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
    public GameObject mapView;

    bool isMapView = false;

    // Start is called before the first frame update
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isMapView = true;
            mapView.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            isMapView = false;
            mapView.SetActive(false);
        }
    }
}
