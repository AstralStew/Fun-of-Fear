using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Paperticket {
    public class GhostMovement : MonoBehaviour
    {

        [Header("References")]

        [SerializeField] private Transform WaypointsParent;
        private List<Transform> waypoints = new List<Transform>();

        [Header("Controls")]

        [SerializeField] private float minWaitDuration;
        [SerializeField] private float maxWaitDuration;
        [SerializeField] private int checkFrequency;        // How long to wait between checks to see if we've reached our destination
        [SerializeField] private float pauseAfterTurning;   // How long to wait after turning towards a new wandering path
        [SerializeField] private bool debugging;

        [Header("Read Only")]

        [SerializeField] private Vector3 currentTarget;
        [SerializeField] private float smoothing;

        private int previousIndex;
        private NavMeshAgent agent;


        // Start is called before the first frame update
        void Start() {
            // Grab the nav agent reference
            agent = agent ?? GetComponent<NavMeshAgent>();
            if (!agent) {
                Debug.LogError("[GhostMovement] ERROR -> No NavMeshAgent found on this object! Please add one. Disabling object.");
                gameObject.SetActive(false);
            }

            // Grab the WaypointsParent reference
            WaypointsParent = WaypointsParent ?? GameObject.Find("Waypoints").transform;
            if (!WaypointsParent) {
                Debug.LogError("[GhostMovement] ERROR -> No WaypointsParent could be found! Please add one to the scene. Disabling object.");
                gameObject.SetActive(false);
            }

            // Grab the waypoints reference

            foreach (Transform t in WaypointsParent.GetComponentsInChildren<Transform>()) if (t != WaypointsParent) waypoints.Add(t);
            if (waypoints.Count == 0) {
                Debug.LogError("[GhostMovement] ERROR -> No waypoints found under WaypointsParent! Please add children to WaypointsParent. Disabling object.");
                gameObject.SetActive(false);
            }



            //agent.updateRotation = false;



            StartCoroutine(Waiting());
        }


        // Use this for checking to see player later
        //void Update()
        //{
        //    //
        //}



        public void PickNewWaypoint() {

            if (debugging) Debug.Log("[GhostMovement] Picking new waypoint..");


            // Pick a new waypoint, must be different from the previous one
            int newIndex = previousIndex;
            while (newIndex == previousIndex) {
                newIndex = Random.Range(0, waypoints.Count - 1);
            }
            previousIndex = newIndex;

            if (debugging) Debug.Log("[GhostMovement] Picked " + waypoints[newIndex] + "!");

            //Turn off the navmesh agents own rotation
            //agent.updatePosition = false;
            //agent.updateRotation = false;

            // Set the waypoint as the next destination
            currentTarget = waypoints[newIndex].position;
            agent.SetDestination(currentTarget);

            // Rotate towards the new destination
            StartCoroutine(TurningToTargetDirection());


        }

        IEnumerator TurningToTargetDirection() {

            yield return null;
            agent.isStopped = true;

            if (debugging) Debug.Log("[GhostMovement] Turning to target direction..");

            // Get the rotation to face the target direction        
            while (!agent.hasPath) {
                Debug.Log("[GhostMovement] (waiting for path)");
                yield return null;
            }
            Quaternion lookRotation = Quaternion.LookRotation((agent.path.corners[1] - transform.position).normalized, Vector3.up);
            //agent.steeringTarget
            //agent.velocity
            //agent.path.corners[1]

            if (debugging) Debug.Log("[GhostMovement] Look rotation = " + lookRotation.eulerAngles);

            //transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, agent.angularSpeed * Time.deltaTime * 0.05f);

            float initialAngle = Quaternion.Angle(transform.rotation, lookRotation);
            // Continue once the angle to the target direction is acceptable
            while (Quaternion.Angle(transform.rotation, lookRotation) > 0.5) {

                if (debugging) Debug.Log("[GhostMovement] Initial angle = " + initialAngle);
                if (debugging) Debug.Log("[GhostMovement] Current angle = " + Quaternion.Angle(transform.rotation, lookRotation));

                // t = 0 -> 1
                // smoothing = Mathf.SmoothStep(minDelta, maxDelta, currentAngle / initialAngle)
                // rotatetowards (current, target, smoothing * time.deltaTime)


                //float currentAngle = Quaternion.Angle(transform.rotation, lookRotation);
                //smoothing = Mathf.SmoothStep(30, 60, (1 - currentAngle / initialAngle));

                smoothing = agent.angularSpeed;

                //if (debugging) Debug.Log("[GhostMovement] 1 - CA / IA = " + (1 - currentAngle / initialAngle));

                // Rotate towards the target direction
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, smoothing * Time.deltaTime);

                yield return null;
            }





            // Brief pause before start moving
            yield return new WaitForSeconds(pauseAfterTurning);

            if (debugging) Debug.Log("[GhostMovement] Rotated successfully!");

            agent.isStopped = false;

            // Start wandering towards the new target
            StartCoroutine(Wandering());

        }


        IEnumerator Wandering() {

            if (debugging) Debug.Log("[GhostMovement] Wandering..");

            //agent.updatePosition = true;
            //agent.updateRotation = true;

            // DESTINATION CHECK
            bool arrived = false;
            while (!arrived) {

                // Determine if we are still attempting to calculate a path
                if (!agent.pathPending) {

                    // Are we close enough to the minimum stopping distance?
                    if (agent.remainingDistance <= agent.stoppingDistance) {

                        // Either there's no path left to take, or our speed is close enough to 0 
                        if (!agent.hasPath || Mathf.Approximately(agent.velocity.sqrMagnitude, 0f)) {
                            if (debugging) Debug.Log("[GhostMovement] Success!");
                            arrived = true;


                        } else if (debugging) Debug.Log("[GhostMovement] Agent has path, or velocity is still too high..");
                    } else if (debugging) Debug.Log("[GhostMovement] Remaining distance greater than stopping distance..");
                } else if (debugging) Debug.Log("[GhostMovement] Path is pending..");


                // Wait a while before checking again
                yield return new WaitForSeconds(checkFrequency == 0 ? Time.deltaTime : Mathf.Abs(checkFrequency));
            }

            if (debugging) Debug.Log("[GhostMovement] Wandered successfully!");

            StartCoroutine(Waiting());

        }

        IEnumerator Waiting() {

            if (debugging) Debug.Log("[GhostMovement] Waiting..)");

            // Wait a random amount of time
            yield return new WaitForSeconds(Random.Range(minWaitDuration, maxWaitDuration));

            if (debugging) Debug.Log("[GhostMovement] Waited successfully!");

            PickNewWaypoint();

        }



    }





}
