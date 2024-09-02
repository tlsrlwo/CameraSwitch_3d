using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CST
{
    public class AimStateManager : MonoBehaviour
    {
        [SerializeField] private float mouseSense;
        private float xAxis, yAxis;
        public Transform camFollowPos;

        private void Update()
        {
            xAxis += Input.GetAxisRaw("Mouse X") * mouseSense;
            yAxis -= Input.GetAxisRaw("Mouse Y") * mouseSense;
            yAxis = Mathf.Clamp(yAxis, -80, 80);
        }

        private void LateUpdate()
        {
            camFollowPos.localEulerAngles = new Vector3(yAxis, camFollowPos.localEulerAngles.y, camFollowPos.localEulerAngles.z);
            //rotate player
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, xAxis, transform.eulerAngles.z);
        }


    }
}
