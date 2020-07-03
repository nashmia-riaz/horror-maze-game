using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public GameObject mapView;

    bool isMapView = false;

    public Image batteryFill;

    // Start is called before the first frame update
    void Start()
    {
     
    }

    public void UpdateBattery(float fill)
    {
        if (fill <= 0) return;
        batteryFill.fillAmount = fill;
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
