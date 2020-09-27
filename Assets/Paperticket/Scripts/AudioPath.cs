using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

namespace Paperticket {
    [RequireComponent(typeof(PathCreator))]
    public class AudioPath : MonoBehaviour {


        [Header("References")]

        PathCreator path;
        [SerializeField] BoxCollider pathBounds;

        [SerializeField] Transform audioTransform;
        AudioSource audioSource;




        [Header("Controls")]

        [Tooltip("hefokefok")]
        [SerializeField] [Range(0.1f, 100)] float playerCheckFrequency = 1f;
        [SerializeField] [Range(0.1f, 100)] float pathCheckFrequency = 0.1f;

        [SerializeField] AnimationCurve volumeOverPath;
        [SerializeField] AnimationCurve pitchOverPath;
                
        

        void Awake() {

            path = GetComponent<PathCreator>();

            if (!pathBounds) {
                Debug.LogError("[AudioPath] ERROR -> No Path Bounds could be found! Please add one. Disabling object.");
                gameObject.SetActive(false);
            }

            if (!audioTransform) {
                Debug.LogError("[AudioPath] ERROR -> No Audio Transform could be found! Please add one. Disabling object.");
                gameObject.SetActive(false);
            }
            audioSource = audioTransform.GetComponentInChildren<AudioSource>();
            if (!audioSource) {
                Debug.LogError("[AudioPath] ERROR -> No Audio Source could be found! Please add one. Disabling object.");
                gameObject.SetActive(false);
            }


        }

        private void OnEnable() {
            StartCoroutine(TrackPlayer());
        }
        private void OnDisable() {
            StopAllCoroutines();
        }


        IEnumerator TrackPlayer() {

            float closestTime;

            while (PTUtilities.instance == null) yield return null;

            while (true) {

                if (pathBounds.bounds.Contains(PTUtilities.instance.HeadsetPosition())) {

                    closestTime = path.path.GetClosestTimeOnPath(PTUtilities.instance.HeadsetPosition());
                    //if (reversePath) closestTime = Mathf.Abs(1 - closestTime);
                    
                    audioTransform.position = path.path.GetPointAtTime(closestTime, EndOfPathInstruction.Stop);
                    audioTransform.rotation = path.path.GetRotation(closestTime, EndOfPathInstruction.Stop);

                    audioSource.volume = volumeOverPath.Evaluate(closestTime);
                    audioSource.pitch = pitchOverPath.Evaluate(closestTime);

                    yield return new WaitForSeconds(pathCheckFrequency);

                } else yield return new WaitForSeconds(playerCheckFrequency);                

            }
        }
    }
}