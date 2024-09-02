using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CST
{
    public class IKFootPlacement : MonoBehaviour
    {
        Animator anim;
        public LayerMask layerMask;
        
        [Range(0, 1f)]
        public float DistanceToGround;


        private void Awake()
        {
            anim = GetComponentInChildren<Animator>();
           
        }
        private void Update()
        {
            
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if(anim)
            {
                // only works for humanoid avatars
                anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
                anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1f);

                // Left Foot
                RaycastHit hit;
                Ray ray = new Ray(anim.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);
                if(Physics.Raycast(ray, out hit, DistanceToGround + 1f))
                {
                    if(hit.transform.tag == "Walkable")
                    {
                        Vector3 footPosition = hit.point; //hit.transform.point 를 하면 hit 한 물체의 transform 지점이라 rayHit 지점이랑 다름
                        footPosition.y += DistanceToGround;
                        anim.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);
                    }
                }

            }
        }

    }
}
