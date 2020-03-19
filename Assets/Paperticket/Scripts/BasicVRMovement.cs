using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Paperticket
{
    [RequireComponent(typeof(Rigidbody))]
    public class BasicVRMovement : MonoBehaviour
    {



        [SerializeField] CapsuleCollider capsuleController;

        [SerializeField] float moveSpeed;

        Rigidbody rb;

        Vector2 input;

        Camera camera;

        private void Awake() {
            rb = GetComponent<Rigidbody>();
            camera = GetComponentInChildren<Camera>();
        }

        private void FixedUpdate() {
            float speed = GetInput();

            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = camera.transform.forward * input.y + camera.transform.right * input.x;

            // get a normal for the surface that is being touched to move along it
            Physics.SphereCast(transform.position, capsuleController.radius, Vector3.down, out RaycastHit hitInfo,
                               capsuleController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal);

            Vector3 newPosition = rb.position + (speed * new Vector3(desiredMove.x, 0, desiredMove.z));

            rb.MovePosition(newPosition);
            
        }



        float GetInput() {

            // Read input
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");
            input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (input.sqrMagnitude > 1) {
                input.Normalize();
            }

            return moveSpeed;

        }





    }


}
