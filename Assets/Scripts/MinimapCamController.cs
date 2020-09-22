using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perdita
{
    public class MinimapCamController : MonoBehaviour
    {
        [SerializeField]
        bool rotateWithTarget;

        [SerializeField]
        float distanceFromTarget;
        public Transform player;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void LateUpdate()
        {
            Vector3 newPosition = player.position;
            newPosition.y = distanceFromTarget;
            transform.position = newPosition;

            if(rotateWithTarget)
                transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
        }
    }
}