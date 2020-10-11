using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for rock. Mainly exists to play sound of rock hitting the floor in 3D space.
/// </summary>
namespace Perdita
{
    public class StoneController : MonoBehaviour
    {
        AudioSource audioSource;

        public AudioClip[] stoneSounds;
        private void Start()
        {
            audioSource = gameObject.GetComponent<AudioSource>();
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Ground")
            {
                GameController.instance.SetDistraction(transform.position);
                audioSource.volume = GameController.instance.volume;
                audioSource.PlayOneShot(stoneSounds[Random.Range(0, stoneSounds.Length)]);
            }
        }

    }
}