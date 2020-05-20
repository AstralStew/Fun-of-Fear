using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityStandardAssets.CrossPlatformInput;

namespace Paperticket
{
    [RequireComponent(typeof(Rigidbody))]
    public class BasicVRMovement : MonoBehaviour {

        [Header("References")]

        [SerializeField] CapsuleCollider capsuleCollider;

        [Header("Controls")]

        [SerializeField] LayerMask groundLayers;

        //[SerializeField] float camSpeed;
        [SerializeField] float moveSpeed;

        public bool UseGravity {
            get {
                return rb.useGravity;
            }
            set {
                rb.useGravity = value;
            }
        }

        Rigidbody rb;

        Vector3 finalInput;
        Vector2 leftInput;
        Vector2 rightInput;

        Camera headCamera;
        XRRig playerRig;

        bool setupComplete;

        private void Awake() {
            StartCoroutine(Setup());
        }
        IEnumerator Setup() {
                        
            while (rb == null) {
                rb = GetComponent<Rigidbody>();
                yield return null;
            }

            while (headCamera == null) {
                headCamera = PTUtilities.instance.headProxy.GetComponent<Camera>(); //GetComponentInChildren<Camera>();
                yield return null;
            }

            while (playerRig == null) {
                playerRig = PTUtilities.instance.playerRig; 
                yield return null;
            }

            capsuleCollider = capsuleCollider ?? GetComponent<CapsuleCollider>();

            setupComplete = true;
        }

        private void FixedUpdate() {
            if (!setupComplete) return;

            // Get input and set speed
            GetInput();
            float speed = moveSpeed;

            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = (headCamera.transform.forward * finalInput.y) + (headCamera.transform.right * finalInput.x);

            // get a normal for the surface that is being touched to move along it
            Physics.SphereCast(transform.position, capsuleCollider.radius, Vector3.down, out RaycastHit hitInfo, capsuleCollider.height / 2f, groundLayers, QueryTriggerInteraction.Ignore);

            // Create a vector that moves along the surface in the desired direction
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal);

            Vector3 newPosition = rb.position + (speed * new Vector3(desiredMove.x, 0, desiredMove.z));

            rb.MovePosition(newPosition);
            
        }

        //void LateUpdate() {
            

            //playerRig.transform.position = Vector3.MoveTowards(playerRig.transform.position, rb.position, finalInput.magnitude * moveSpeed);

        //}



        void GetInput() {

            // Read input
            float leftHorizontal = CrossPlatformInputManager.GetAxis("LeftThumbstickHorizontal");
            float leftVertical = CrossPlatformInputManager.GetAxis("LeftThumbstickVertical");
            float rightHorizontal = CrossPlatformInputManager.GetAxis("RightThumbstickHorizontal");
            float rightVertical = CrossPlatformInputManager.GetAxis("RightThumbstickVertical");

            leftInput = new Vector2(leftHorizontal, leftVertical);
            rightInput = new Vector2(rightHorizontal, rightVertical);

            // normalize left thumbstick input if it exceeds 1 in combined length:
            if (leftInput.sqrMagnitude > 1) {
                leftInput.Normalize();
            }

            // normalize right thumbstick input if it exceeds 1 in combined length:
            if (rightInput.sqrMagnitude > 1) {
                rightInput.Normalize();
            }

            finalInput = leftInput + rightInput;
            
        }



        



    }


}
