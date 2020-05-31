using System;
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

        [SerializeField] float moveSpeed;

        [SerializeField] float groundCastHeight;
        [SerializeField] LayerMask groundLayers;
        [SerializeField] float minBodyHeight = 0.5f;
        [SerializeField] float maxBodyHeight = 2f;
        [SerializeField] float bodyHeightOffset = 0.05f;

        [Space(10)]
        [SerializeField] bool debugging;
        [SerializeField] bool framedebugging;

        [Header("Read Only")]

        //[SerializeField] Vector3 groundPos;

        [SerializeField] Vector3 finalInput;
        Vector2 leftInput;
        Vector2 rightInput;

        public bool UseGravity {
            get {
                return rb.useGravity;
            }
            set {
                rb.useGravity = value;
            }
        }
        public bool AllowMovement;


        Rigidbody rb;
        Camera headCamera;
        XRRig playerRig;        

        bool setupComplete;

        private void Awake() {
            StartCoroutine(Setup());
        }
        IEnumerator Setup() {
            if (debugging) Debug.Log("[BasicVRMovement] Entering setup...");

            while (rb == null) {
                if (framedebugging) Debug.Log("[BasicVRMovement] Trying to find rigidbody...");
                rb = GetComponent<Rigidbody>();
                yield return null;
            }
            if (framedebugging) Debug.Log("[BasicVRMovement] Rigidbody found!");

            while (headCamera == null) {
                if (framedebugging) Debug.Log("[BasicVRMovement] Trying to find head camera...");
                headCamera = PTUtilities.instance.headProxy.GetComponent<Camera>(); 
                yield return null;
            }
            if (framedebugging) Debug.Log("[BasicVRMovement] Head camera found!");

            while (playerRig == null) {
                if (framedebugging) Debug.Log("[BasicVRMovement] Trying to find player rig...");
                playerRig = PTUtilities.instance.playerRig; 
                yield return null;
            }
            if (framedebugging) Debug.Log("[BasicVRMovement] Player rig found!");

            capsuleCollider = capsuleCollider ?? GetComponent<CapsuleCollider>();

            setupComplete = true;

            if (debugging) Debug.Log("[BasicVRMovement] Setup complete!");
        }

        private void FixedUpdate() {
            if (!setupComplete || !AllowMovement) return;

            if (framedebugging) Debug.Log("[BasicVRMovement] Running fixed update.");

            // Get input and set speed
            GetInput();

            // Move the player using rigibody
            MovePlayer();

            // Resize the body collider
            ResizeBody();

            if (framedebugging) Debug.Log("[BasicVRMovement] Finished fixed update.");
        }

        //void LateUpdate() {
            

            //playerRig.transform.position = Vector3.MoveTowards(playerRig.transform.position, rb.position, finalInput.magnitude * moveSpeed);

        //}



        void GetInput() {
            if (framedebugging) Debug.Log("[BasicVRMovement] Entering GetInput...");

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

            if (framedebugging) Debug.Log("[BasicVRMovement] GetInput complete!");
        }


        void MovePlayer() {
            if (framedebugging) Debug.Log("[BasicVRMovement] Entering MovePlayer...");

            float speed = moveSpeed;

            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = (headCamera.transform.forward * finalInput.y) + (headCamera.transform.right * finalInput.x);

            // get a normal for the surface that is being touched to move along it
            Vector3 capsulePos = capsuleCollider.transform.position + capsuleCollider.center;
            Physics.SphereCast(capsulePos, capsuleCollider.radius, Vector3.down, out RaycastHit hitInfo, groundCastHeight, groundLayers, QueryTriggerInteraction.Ignore);
            if (framedebugging) Debug.Log("[BasicVRMovement] SphereCast origin = " + capsulePos + Environment.NewLine + 
                                          (hitInfo.collider ? ("[BasicVRMovement] SphereCast hit result = " + hitInfo.collider.gameObject + Environment.NewLine ) : "" )+ 
                                          "SphereCast hit point = " + hitInfo.point + Environment.NewLine + 
                                          "SphereCast normal = " + hitInfo.normal);

            // Create a vector that moves along the surface in the desired direction
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal);

            // Save out the ground position for other calculations
            //groundPos = hitInfo.point;

            // Move to a new position based on all the above
            Vector3 newPosition = rb.position + (speed * new Vector3(desiredMove.x, 0, desiredMove.z));
            rb.MovePosition(newPosition);

            if (framedebugging) Debug.Log("[BasicVRMovement] MovePlayer complete!");
        }

        
        void ResizeBody() {
            if (framedebugging) Debug.Log("[BasicVRMovement] Entering ResizeBody...");
            
            float height = capsuleCollider.height = playerRig.cameraInRigSpaceHeight;


            // Set the body height and center
            capsuleCollider.height = height;
            capsuleCollider.center = playerRig.cameraInRigSpacePos - new Vector3(0, height / 2, 0);
            
            if (framedebugging) Debug.Log("[BasicVRMovement] Body clamped height = " + capsuleCollider.height + Environment.NewLine +
                                          "[BasicVRMovement] Body  height = " + height + Environment.NewLine +
                                          "[BasicVRMovement] Body center = " + capsuleCollider.center);

            if (framedebugging) Debug.Log("[BasicVRMovement] ResizeBody complete!");
        }


    }


}



//Vector3 camPos = headCamera.transform.position;

// The height of the capsule collider is restricted to the bottom of the play area
// This allows the player to walk off surfaces without their body stretching to the ground

//float height = Mathf.Max(groundPos.y, playerRig.transform.position.y);
//float clampedHeight = Mathf.Clamp(camPos.y - height + bodyHeightOffset, minBodyHeight, maxBodyHeight);            //Vector3 clampedHeight = (groundPos.y > playerRig.transform.position.y) ? groundPos : playerRig.transform.position;


//capsuleCollider.height = clampedHeight;