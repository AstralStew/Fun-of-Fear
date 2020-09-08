using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Paperticket {
    public class GhostPerception : MonoBehaviour
    {
        [Header("References")]

        [SerializeField] private Transform sightTransform;

        [Header("Controls")]
        
        [SerializeField] private float sightRange;
        [SerializeField] private float sightMaxAngle;
        [SerializeField] private float sightSphereRadius;
        [SerializeField] private LayerMask targetLayers;
        [SerializeField] private float sightCheckFrequency;         // How long to wait between checks to see if we can see the player
        [SerializeField] private float searchingDuration;           // How long to continue searching for the player after losing them
        [SerializeField] private float reachRange;                  // How close the player must be to be caught by the ghost

        [SerializeField] private bool debugging;
        [SerializeField] private bool framedebugging;

        [Header("Read Only")]

        [SerializeField] private Transform targetPlayer;
        [SerializeField] private bool canSeePlayer;
        [SerializeField] bool hasReachedPlayer;
        public bool HasReachedPlayer { get { return hasReachedPlayer; } }

        private Vector3 lastSeenPlayerPos;                          // The position of the player the last time we saw them
        public Vector3 LastPlayerPosition {
            get { return lastSeenPlayerPos; }
        }
        public Vector3 RealPlayerPosition {
            get { return targetPlayer.position; }
        }

        public Vector3 RealDirectionToPlayer {
            get { return targetPlayer.position - sightTransform.position; }
        }

        // Public events

        public delegate void OnSeePlayer();
        public event OnSeePlayer onSeePlayer;

        public delegate void OnLosePlayer();
        public event OnLosePlayer onLosePlayer;

        public delegate void OnForgottenPlayer();
        public event OnForgottenPlayer onForgottenPlayer;

        public delegate void OnReachPlayer();
        public event OnReachPlayer onReachPlayer;

        private Coroutine searchingCoroutine;


        public UnityEvent SeePlayerEvents;


        // Start is called before the first frame update
        void Awake() {
            // Grab the sight transform reference            
            if (!sightTransform) {
                Debug.LogError("[GhostPerception] ERROR -> No SightTransform found on this object! Please add one. Disabling object.");
                gameObject.SetActive(false);
            }

            StartCoroutine(Setup());

        }
        IEnumerator Setup() {
            if (debugging) Debug.Log("[GhostPerception] Starting setup...");

            // Grab the player reference            
            while (!targetPlayer) {
                if (framedebugging) Debug.Log("[GhostPerception] Searching for player object...");
                targetPlayer = PTUtilities.instance.headProxy;
                yield return null;
            }
            if (framedebugging) Debug.Log("[GhostPerception] Player found!");

            hasReachedPlayer = false;

            if (debugging) Debug.Log("[GhostPerception] Completed setup!");

            StartCoroutine(CheckForPlayer());

        }



        //private void OnTriggerEnter( Collider other ) {

        //    // Make a perception check if a player collider is detected
        //    if (targetLayers == (targetLayers | (1 << other.gameObject.layer))) {                

        //    }
        //}


        /// PERCEPTION FUNCTIONS        
        //IEnumerator CheckForPlayer() {
        //    Vector3 dir;
        //    Color col = Color.red;
            
        //    while (!hasReachedPlayer) {
                
        //        // Bail if the player is too far away
        //        if (canSeePlayer || RealDirectionToPlayer.magnitude <= sightRange) {
        //            if (framedebugging) Debug.Log("[GhostPerception] Current range from player: " + (int)RealDirectionToPlayer.magnitude);

                    
        //            // Bail if the player is outside of the max sight angle
        //            if (canSeePlayer || Vector3.Angle(sightTransform.forward, RealDirectionToPlayer) <= sightMaxAngle) {
        //                if (framedebugging) Debug.Log("[GhostPerception] Current angle to player: " + (int)Vector3.Angle(sightTransform.forward, RealDirectionToPlayer));


        //                // Check if we've reached the player and stop searching
        //                if (canSeePlayer && RealDirectionToPlayer.magnitude <= reachRange) {
        //                    if (debugging) Debug.Log("[GhostPerception] I CAUGHT THE PLAYER!! (^w^)");

        //                    onReachPlayer.Invoke();
        //                    hasReachedPlayer = true;
        //                    SeePlayerEvents.Invoke();
        //                    //canSeePlayer = true;

        //                    if (searchingCoroutine != null) {
        //                        StopCoroutine(searchingCoroutine);
        //                        if (debugging) Debug.Log("[GhostPerception] The 'Searching for lost player' coroutine was running, stopping it now.");
        //                    }
        //                    continue;
        //                }

        //                // Shoot a SphereCast towards the player position (to allow crouching behind things etc.)
        //                RaycastHit raycastHit = new RaycastHit();
        //                if (Physics.SphereCast(sightTransform.position, sightSphereRadius, RealDirectionToPlayer, out raycastHit, sightRange, targetLayers.value)) {

        //                    if (raycastHit.transform.gameObject.layer == LayerMask.NameToLayer("Player")) {

        //                        lastSeenPlayerPos = raycastHit.transform.position;

        //                        // Trigger an event if we are just seeing player
        //                        if (!canSeePlayer) {
        //                            if (debugging) {
        //                                col = Color.green;
        //                                Debug.Log("[GhostPerception] HEY I just saw the player! (0w0)");
        //                            }

        //                            onSeePlayer.Invoke();
        //                            SeePlayerEvents.Invoke();
        //                            canSeePlayer = true;

        //                            if (searchingCoroutine != null) {
        //                                StopCoroutine(searchingCoroutine);
        //                                if (debugging) Debug.Log("[GhostPerception] The 'Searching for lost player' coroutine was running, stopping it now.");
        //                            }
        //                        }

        //                        // Trigger an event if we have just lost the player
        //                    } 
        //                    //else if (canSeePlayer) {
        //                    //    if (debugging) {
        //                    //        col = Color.red;
        //                    //        Debug.Log("[GhostPerception] Damn, I lost the player! (>m<)");
        //                    //    }

        //                    //    onLosePlayer.Invoke();
        //                    //    canSeePlayer = false;

        //                    //    // Start a timer for how long we'll look for the lost player before giving up
        //                    //    if (searchingCoroutine != null) {
        //                    //        StopCoroutine(searchingCoroutine);
        //                    //        if (debugging) Debug.Log("[GhostPerception] The 'Searching for lost player' coroutine was already started, restarting it now.");
        //                    //    }
        //                    //    searchingCoroutine = StartCoroutine(SearchingCoroutine());
        //                    //}
        //                }

        //                // Draw the debug rays
        //                if (debugging) {
        //                    Debug.DrawRay(sightTransform.position, RealDirectionToPlayer * sightRange, col);
        //                    Debug.DrawRay(sightTransform.position + (sightTransform.right * sightSphereRadius), RealDirectionToPlayer * sightRange, col);
        //                    Debug.DrawRay(sightTransform.position - (sightTransform.right * sightSphereRadius), RealDirectionToPlayer * sightRange, col);
        //                    Debug.DrawRay(sightTransform.position + (sightTransform.up * sightSphereRadius), RealDirectionToPlayer * sightRange, col);
        //                    Debug.DrawRay(sightTransform.position - (sightTransform.up * sightSphereRadius), RealDirectionToPlayer * sightRange, col);
        //                }

        //            } else if (framedebugging) Debug.Log("[GhostPerception] Player is outside of the maximum sight angle. (current angle: " + (int)Vector3.Angle(sightTransform.forward, RealDirectionToPlayer) + ")");
        //        } else if (framedebugging) Debug.Log("[GhostPerception] Player is outside of the maximum sight range. (current range: " + (int)RealDirectionToPlayer.magnitude + ")");
        //        // Wait a while before checking again
        //        yield return new WaitForSeconds(sightCheckFrequency == 0 ? Time.deltaTime : Mathf.Abs(sightCheckFrequency));

        //    }

        //}

        IEnumerator CheckForPlayer() {
            Vector3 dir;
            Color col = Color.red;

            while (!canSeePlayer) {
                                
                // Bail if the player is too far away
                if (RealDirectionToPlayer.magnitude <= sightRange) {
                    if (framedebugging) Debug.Log("[GhostPerception] Current range from player: " + (int)RealDirectionToPlayer.magnitude);


                    // Bail if the player is outside of the max sight angle
                    if (Vector3.Angle(sightTransform.forward, RealDirectionToPlayer) <= sightMaxAngle) {
                        if (framedebugging) Debug.Log("[GhostPerception] Current angle to player: " + (int)Vector3.Angle(sightTransform.forward, RealDirectionToPlayer));
                        

                        // Shoot a SphereCast towards the player position (to allow crouching behind things etc.)
                        RaycastHit raycastHit = new RaycastHit();
                        if (Physics.SphereCast(sightTransform.position, sightSphereRadius, RealDirectionToPlayer, out raycastHit, sightRange, targetLayers.value)) {

                            if (raycastHit.transform.gameObject.layer == LayerMask.NameToLayer("Player")) {

                                lastSeenPlayerPos = raycastHit.transform.position;

                                if (debugging) { col = Color.green; Debug.Log("[GhostPerception] HEY I just saw the player! (0w0)"); }

                                onSeePlayer.Invoke();
                                SeePlayerEvents.Invoke();
                                canSeePlayer = true;
                                
                            }
                        }

                        // Draw the debug rays
                        if (debugging) {
                            Debug.DrawRay(sightTransform.position, RealDirectionToPlayer * sightRange, col);
                            Debug.DrawRay(sightTransform.position + (sightTransform.right * sightSphereRadius), RealDirectionToPlayer * sightRange, col);
                            Debug.DrawRay(sightTransform.position - (sightTransform.right * sightSphereRadius), RealDirectionToPlayer * sightRange, col);
                            Debug.DrawRay(sightTransform.position + (sightTransform.up * sightSphereRadius), RealDirectionToPlayer * sightRange, col);
                            Debug.DrawRay(sightTransform.position - (sightTransform.up * sightSphereRadius), RealDirectionToPlayer * sightRange, col);
                        }

                    } //else if (framedebugging) Debug.Log("[GhostPerception] Player is outside of the maximum sight angle. (current angle: " + (int)Vector3.Angle(sightTransform.forward, RealDirectionToPlayer) + ")");
                } //else if (framedebugging) Debug.Log("[GhostPerception] Player is outside of the maximum sight range. (current range: " + (int)RealDirectionToPlayer.magnitude + ")");

                // Wait a while before checking again
                if (sightCheckFrequency <= Time.deltaTime) yield return null;
                else yield return new WaitForSeconds(sightCheckFrequency);

            }

            while (!hasReachedPlayer) {
                //// Check if we've reached the player and stop searching
                if (RealDirectionToPlayer.magnitude <= reachRange) {
                    if (debugging) Debug.Log("[GhostPerception] I CAUGHT THE PLAYER!! (^w^)");

                    onReachPlayer.Invoke();
                    hasReachedPlayer = true;
                    SeePlayerEvents.Invoke();
                    
                }
                yield return null;
            }

        }

        //IEnumerator SearchingCoroutine() {

        //    if (debugging) Debug.Log("[GhostPerception] Searching to find the player I lost! (>->)");

        //    // Spend an amount of time still searching for the player
        //    yield return new WaitForSeconds(searchingDuration);

        //    // Trigger an event to signal that we have given up on the player
        //    onForgottenPlayer.Invoke();
        //    lastSeenPlayerPos = Vector3.zero;

        //    if (debugging) Debug.Log("[GhostPerception] I couldn't find the player I lost, giving up (.__.) ");

        //}

    }

}
