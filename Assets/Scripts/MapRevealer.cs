using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script to reveal map as player walks through it.
/// </summary>
namespace Perdita
{
    public class MapRevealer : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
                Destroy(this.gameObject);
        }

    }
}