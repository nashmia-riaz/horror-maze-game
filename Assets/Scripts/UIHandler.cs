using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Perdita
{
    public class UIHandler : MonoBehaviour
    {
        public GameObject mapView;

        bool isMapView = false;

        public Image batteryFill;
        public Image healthFill;

        public Animator UIAnimator;

        public Text timer;

        public GameObject pausePanel, gameOverPanel, winPanel;

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

        public void RedFlash() {
            UIAnimator.SetTrigger("RedFlash");
        }

        public void UpdateHealth(float fill)
        {
            healthFill.fillAmount = fill;
        }

        public void UpdateTimer(float time)
        {
            int minutes = 0;
            int seconds = (int) time;

            if(seconds > 60)
            {
                minutes = seconds / 60;
                seconds = seconds % 60;
            }

            string timeElapsed = "";

            if (minutes < 10) timeElapsed += "0" + minutes + ":";
            else timeElapsed += minutes + ":";

            if (seconds < 10) timeElapsed += "0" + seconds;
            else timeElapsed += seconds;

            timer.text = timeElapsed;
        }

        public void Pause(bool isPaused)
        {
            pausePanel.SetActive(isPaused);
        }

        public void GameOver()
        {
            gameOverPanel.SetActive(true);
        }

        public void Win()
        {
            winPanel.SetActive(true);
        }
    }
}