using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Paperticket
{
    [RequireComponent(typeof(Rigidbody))]
    public class BasicVRMovement : MonoBehaviour {

        [SerializeField] CapsuleCollider capsuleController;

        [SerializeField] float moveSpeed;

        Rigidbody rb;
        float LastMoveVelocity;

        Vector3 finalInput;
        Vector2 leftInput;
        Vector2 rightInput;

        Camera camera;

        private void Awake() {
            rb = GetComponent<Rigidbody>();
            camera = GetComponentInChildren<Camera>();
        }

        private void FixedUpdate() {
            float speed = GetInput();

            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = camera.transform.forward * finalInput.y + camera.transform.right * finalInput.x;

            // get a normal for the surface that is being touched to move along it
            Physics.SphereCast(transform.position, capsuleController.radius, Vector3.down, out RaycastHit hitInfo,
                               capsuleController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal);

            Vector3 newPosition = rb.position + (speed * new Vector3(desiredMove.x, 0, desiredMove.z));

            rb.MovePosition(newPosition);
            
        }



        float GetInput() {

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

            //Mathf.MoveTowards(rb.velocity.magnitude, moveSpeed, moveSpeed / 10);

            return moveSpeed; // Mathf.MoveTowards(rb.velocity.magnitude, moveSpeed, moveSpeed / 40); ;

        }





    }


}
