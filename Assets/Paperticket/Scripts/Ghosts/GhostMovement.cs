using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

namespace Paperticket {
    public class GhostMovement : MonoBehaviour
    {

        [Header("References")]

        private GhostPerception ghostPerception;
        [SerializeField] private Transform WaypointsParent;
        private List<Transform> waypoints = new List<Transform>();

        [SerializeField] private bool debugging;
        [SerializeField] private bool framedebugging;

        [Header("Wandering Controls")]

        [SerializeField] private float wanderingSpeed;
        [SerializeField] private float wanderingAngularSpeed;

        [SerializeField] private float minWaitDuration;
        [SerializeField] private float maxWaitDuration;
        [SerializeField] private float destinationCheckFreq;        // How long to wait between checks to see if we've reached our destination
        [SerializeField] private float wanderingTurnPauseDuration;           // How long to wait after turning towards a new wandering path

        [Header("Chasing Controls")]

        [SerializeField] private float chasingSpeed;
        [SerializeField] private float chasingAngularSpeed;
        [SerializeField] private float chasingTurnPauseDuration;           // How long to wait after turning towards a new wandering path
        [SerializeField] private float turnPauseVariance;

        [SerializeField] private float playerPosCheckFreq;          // How long to wait between checks to save the players position while chasing


        [Header("Read Only")]

        [SerializeField] private Vector3 currentTarget;
        [SerializeField] private float smoothing;


        private int previousIndex;
        private NavMeshAgent agent;

        private Coroutine currentCoroutine;
        private bool lostPlayer;


        // Start is called before the first frame update
        void Awake() {
            // Grab the nav agent reference
            ghostPerception = ghostPerception ?? GetComponent<GhostPerception>() ?? GetComponentInParent<GhostPerception>() ?? GetComponentInChildren<GhostPerception>();
            if (!ghostPerception) {
                Debug.LogError("[GhostMovement] ERROR -> No GhostPerception found on this object! Please add one. Disabling object.");
                gameObject.SetActive(false);
            }

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

            // Grab reference to each individual waypoint
            foreach (Transform t in WaypointsParent.GetComponentsInChildren<Transform>()) if (t != WaypointsParent) waypoints.Add(t);
            if (waypoints.Count == 0) {
                Debug.LogError("[GhostMovement] ERROR -> No waypoints found under WaypointsParent! Please add children to WaypointsParent. Disabling object.");
                gameObject.SetActive(false);
            }

            //// Grab the nav agent reference            
            //if (!sightTransform) {
            //    Debug.LogError("[GhostMovement] ERROR -> No SightTransform found on this object! Please add one. Disabling object.");
            //    gameObject.SetActive(false);
            //}

        }

        private void OnEnable() {
            ghostPerception.onSeePlayer += StartChasingPlayer;
            ghostPerception.onLosePlayer += StartSearchingForPlayer;
            ghostPerception.onForgottenPlayer += StartWandering;
            ghostPerception.onReachPlayer += StartScaringPlayer;
        }
        private void OnDisable() {
            ghostPerception.onSeePlayer -= StartChasingPlayer;
            ghostPerception.onLosePlayer -= StartSearchingForPlayer;
            ghostPerception.onForgottenPlayer -= StartWandering;
            ghostPerception.onReachPlayer -= StartScaringPlayer;
        }

        void Start() {
            StartCoroutine(Waiting());
        }




        /// CHASING FUNCTIONS

        void StartChasingPlayer() {
            if (debugging) Debug.Log("[GhostMovement] Aight, I'm gonna start chasing the player now (^w^) ");

            lostPlayer = false;

            //if (currentCoroutine != null) {
            //    StopCoroutine(currentCoroutine);
            //}

            StopAllCoroutines();

            currentCoroutine = StartCoroutine(Chasing());

        }
        void StartSearchingForPlayer() {
            if (debugging) Debug.Log("[GhostMovement] Gotta try and catch up to where they were! (>_<) ");

            //if (currentCoroutine != null) {
            //    lostPlayer = true;
            //    StopCoroutine(currentCoroutine);
            //}

            lostPlayer = true;
            StopAllCoroutines();

            currentCoroutine = StartCoroutine(Searching());
        }
        void StartWandering() {
            if (debugging) Debug.Log("[GhostMovement] I'm going back to wandering, this is boring (>->) ");

            StopAllCoroutines();
            lostPlayer = false;
            PickNewWaypoint();
        }
      
        IEnumerator Chasing() {
                       
            agent.isStopped = false;
            agent.speed = chasingSpeed;
            agent.angularSpeed = chasingAngularSpeed;
            agent.autoBraking = false;

            while (!lostPlayer) {
                
                if (currentTarget != ghostPerception.LastPlayerPosition) {
                    agent.SetDestination(ghostPerception.LastPlayerPosition);

                }
                currentTarget = ghostPerception.LastPlayerPosition;
                               
                // Wait a while before checking again
                yield return new WaitForSeconds(playerPosCheckFreq == 0 ? Time.deltaTime : Mathf.Abs(playerPosCheckFreq));
            }
            
        }
        IEnumerator Searching() {
                        
            agent.SetDestination(ghostPerception.LastPlayerPosition);
            agent.autoBraking = true;

            bool reachedLastSeenPos = false;
            bool turnedTowardsPlayer = false;
            float t = 0;
            float timerSavePlayerPos = 0;
            Vector3 guidingPos = Vector3.zero;
            Coroutine waitCo = null;
            while (lostPlayer) {
                yield return null;

                // Save the player's position 1 seconds after losing them
                if (!turnedTowardsPlayer && guidingPos == Vector3.zero) {
                    timerSavePlayerPos += Time.deltaTime;
                    if (timerSavePlayerPos > 1f) {
                        guidingPos = ghostPerception.RealPlayerPosition;
                    }
                }

                // Move to the last known player position                
                if (!reachedLastSeenPos) {
                    t += Time.deltaTime;
                    if (t > (destinationCheckFreq == 0 ? Time.deltaTime : Mathf.Abs(destinationCheckFreq))) {
                        t = 0;
                        // Determine if we are still attempting to calculate a path
                        if (!agent.pathPending) {
                            // Are we close enough to the minimum stopping distance?
                            if (agent.remainingDistance <= agent.stoppingDistance) {
                                // Either there's no path left to take, or our speed is close enough to 0 
                                if (!agent.hasPath || Mathf.Approximately(agent.velocity.sqrMagnitude, 0f)) {
                                    if (debugging) Debug.Log("[GhostMovement] Reached the last known player position...");
                                    reachedLastSeenPos = true;
                                    continue;
                                    
                                } else if (framedebugging) Debug.Log("[GhostMovement] Agent has path, or velocity is still too high..");
                            } else if (framedebugging) Debug.Log("[GhostMovement] Remaining distance greater than stopping distance..");
                        } else if (framedebugging) Debug.Log("[GhostMovement] Path is pending..");
                    }

                // Look around to see if we can find the player
                } else {

                    // Turn towards the direction the player headed
                    if (!turnedTowardsPlayer) {

                        // Rotate to face the player's heading
                        agent.SetDestination(guidingPos);
                        yield return StartCoroutine(TurningToTargetDirection());
                        if (debugging) Debug.Log("[GhostMovement] Haha gotcha... Wait, they aren't here! (OxO)' ");

                        // Set the agent back to wandering speed
                        agent.speed = wanderingSpeed;
                        agent.angularSpeed = wanderingAngularSpeed;

                        turnedTowardsPlayer = true;                    
                    }
                    
                    // Pause for a few seconds
                    if (debugging) Debug.Log("[GhostMovement] Hmm, where did the player go...?");
                    yield return new WaitForSeconds(chasingTurnPauseDuration + Random.Range(-turnPauseVariance, turnPauseVariance));

                    // Rotate to face a random direction
                    agent.SetDestination(new Vector3(transform.position.x + Random.Range(-1f, 1f), transform.position.y, transform.position.z + Random.Range(-1f, 1f)));

                    if (debugging) Debug.Log("[GhostMovement] Maybe if I turn towards here...?");
                    yield return StartCoroutine(TurningToTargetDirection());                    

                }
                
            }
        }
        

        /// WANDERING FUNCTIONS
        void PickNewWaypoint() {

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
            currentCoroutine = StartCoroutine(TurnToWaypoint());


        }
                
        IEnumerator TurnToWaypoint() {

            yield return null;
            yield return StartCoroutine(TurningToTargetDirection());

            // Brief pause before start moving
            yield return new WaitForSeconds(wanderingTurnPauseDuration);
                       
            // Start wandering towards the new target
            currentCoroutine = StartCoroutine(Wandering());

        }
        IEnumerator Wandering() {

            if (debugging) Debug.Log("[GhostMovement] Wandering..");

            //agent.updatePosition = true;
            //agent.updateRotation = true;

            agent.isStopped = false;
            agent.speed = wanderingSpeed;
            agent.angularSpeed = wanderingAngularSpeed;
            agent.autoBraking = true;

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


                        } //else if (debugging) Debug.Log("[GhostMovement] Agent has path, or velocity is still too high..");
                    } //else if (debugging) Debug.Log("[GhostMovement] Remaining distance greater than stopping distance..");
                } //else if (debugging) Debug.Log("[GhostMovement] Path is pending..");


                // Wait a while before checking again
                yield return new WaitForSeconds(destinationCheckFreq == 0 ? Time.deltaTime : Mathf.Abs(destinationCheckFreq));
            }

            if (debugging) Debug.Log("[GhostMovement] Wandered successfully!");

            currentCoroutine = StartCoroutine(Waiting());

        }
        IEnumerator Waiting() {

            if (debugging) Debug.Log("[GhostMovement] Waiting..)");

            // Wait a random amount of time
            yield return new WaitForSeconds(Random.Range(minWaitDuration, maxWaitDuration));

            if (debugging) Debug.Log("[GhostMovement] Waited successfully!");

            PickNewWaypoint();

        }


        /// REACH PLAYER FUNCTIONS

        void StartScaringPlayer() {
            if (debugging) Debug.Log("[GhostMovement] I have caught the player, time to scare them!! (nwn)");

            agent.isStopped = true;
            agent.speed = 0;
            agent.angularSpeed = wanderingAngularSpeed;
            agent.autoBraking = false;
            
            StopAllCoroutines();

            currentCoroutine = StartCoroutine(ScaringPlayer());

        }

        IEnumerator ScaringPlayer() {
            
            // Rotate to face the player's heading
            agent.SetDestination(ghostPerception.RealPlayerPosition);
            yield return StartCoroutine(TurningToTargetDirection());
            if (debugging) Debug.Log("[GhostMovement] Finished turning to player!");

            // Remain stopped until the escape animations has played
            
        }




        /// UNIVERSAL FUNCTIONS

        IEnumerator TurningToTargetDirection() {

            agent.isStopped = true;

            if (debugging) Debug.Log("[GhostMovement] Turning to target direction..");

            // Get the rotation to face the target direction        
            while (!agent.hasPath) {
                if (framedebugging) Debug.Log("[GhostMovement] (waiting for path)");
                yield return null;
            }
            Quaternion lookRotation = Quaternion.LookRotation((agent.path.corners[1] - transform.position).normalized, Vector3.up);

            if (debugging) Debug.Log("[GhostMovement] Look rotation = " + lookRotation.eulerAngles);

            float initialAngle = Quaternion.Angle(transform.rotation, lookRotation);

            // Continue once the angle to the target direction is acceptable
            while (Quaternion.Angle(transform.rotation, lookRotation) > 0.5) {

                if (framedebugging) Debug.Log("[GhostMovement] Initial angle = " + initialAngle);
                if (framedebugging) Debug.Log("[GhostMovement] Current angle = " + Quaternion.Angle(transform.rotation, lookRotation));

                smoothing = agent.angularSpeed;

                // Rotate towards the target direction
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, smoothing * Time.deltaTime);

                yield return null;
            }

            if (debugging) Debug.Log("[GhostMovement] Rotated successfully!");

        }

    }





}
