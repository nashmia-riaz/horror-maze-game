using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script handles further input from player and manages their variables such as health.
/// </summary>
namespace Perdita
{
    public class PlayerController : MonoBehaviour
    {
        public float health;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                GameController.instance.ToggleBattery();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                GameController.instance.ChargeBattery(10);
            }
            else if (Input.GetMouseButtonDown(0))
            {
                GameController.instance.ThrowRock();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("Player collided with " + collision.gameObject);
            if (collision.gameObject.tag == "Ground")
            {
                GameController.instance.PlayerCollidedWithFloor(transform.position);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "End Point")
            {
                GameController.instance.EndGame();
            }
        }

        public void DoDamage(int damage)
        {
            if (health <= 0) return;

            Debug.Log("Player took damage: "+health);
            health -= damage;
            if (health <= 0) { 
                health = 0;
                GameController.instance.GameOver();
            }
        }
    }
}