using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perdita
{
    public class PlayerController : MonoBehaviour
    {
        public float health;

        // Start is called before the first frame update
        void Start()
        {

        }

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
                Debug.Log("Throwing a rock");
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

        public void DoDamage()
        {
            health -= 10;
            if (health <= 0) health = 0;
        }
    }
}