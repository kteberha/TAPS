using System;
using UnityEngine;

namespace UnityStandardAssets._2D
{
    public class Camera2DFollow : MonoBehaviour
    {
        private Transform target;
        public float damping = 1;
        public float yAddValue = 0f;

        public float yMinValue = 0f;
        public float yMaxValue = 5f; //For the top of the arena
        public float xMinValue = -1f; //For the left wall
        public float xMaxValue = 999999f; //For the right wall

        public Material arrowMat;
        public float dissolveThreshold;

        bool dissolved = true;

        private Camera cam;

		float nextTimeToSearch = 0;

        private float m_OffsetZ;
        private Vector3 m_LastTargetPosition;
        public Vector3 m_CurrentPosition;
        private Vector3 m_CurrentVelocity;
		private float tarY;
		//private bool moving = false;

        private PlayerController playerController;

        // Use this for initialization
        private void Start()
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
            m_LastTargetPosition = target.position;
            m_CurrentPosition = transform.position;
            m_OffsetZ = (m_CurrentPosition - target.position).z;
            transform.SetParent(null);  //transform.parent = null;
			cam = GetComponent<Camera> ();
            playerController = target.GetComponent<PlayerController>();
        }


        // Update is called once per frame
        private void FixedUpdate()
        {
			if (target == null) {
				FindPlayer ();
				return;
			}

            float xMoveDelta = (target.position - m_LastTargetPosition).x;
            float yMoveDelta = (target.position - m_LastTargetPosition).y;
            float zMoveDelta = (target.position - m_LastTargetPosition).z;

            m_OffsetZ = playerController.DetermineCameraZ();

            //handle the direction arrow fading in and out
            if(playerController.lerpPerc < dissolveThreshold)
            {
                if(dissolved)
                {
                    print("arrow is appearing");
                    float currentF = arrowMat.GetFloat("_Dissolve");
                    currentF += Time.deltaTime;
                    arrowMat.SetFloat("_Dissolve", currentF);
                    if (currentF >= 1f)
                        dissolved = false;
                }
            }
            else
            {
                if(!dissolved)
                {
                    print("arrow is disappearing");
                    float currentF = arrowMat.GetFloat("_Dissolve");
                    currentF -= Time.deltaTime;
                    arrowMat.SetFloat("_Dissolve", currentF);
                    if (currentF <= 0f)
                        dissolved = true;
                }
            }


            Vector3 aheadTargetPos = target.position + new Vector3(0, 0, 0) + Vector3.forward*m_OffsetZ;
            Vector3 newPos;
            newPos = Vector3.SmoothDamp(m_CurrentPosition, aheadTargetPos, ref m_CurrentVelocity, damping);



            m_CurrentPosition = newPos;
            transform.position = newPos + new Vector3(0, yAddValue, 0);
            m_LastTargetPosition = newPos;
        }

		void FindPlayer () {
			if (nextTimeToSearch <= Time.time) {
				GameObject searchResult = GameObject.FindGameObjectWithTag ("Player");
				if (searchResult != null)
					target = searchResult.transform;
				nextTimeToSearch = Time.time + 0.5f;
			}
		}
    }
}
