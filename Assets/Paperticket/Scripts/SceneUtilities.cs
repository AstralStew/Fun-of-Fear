using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

namespace Paperticket {

    public class SceneUtilities : MonoBehaviour {

        public static SceneUtilities instance = null;

        public delegate void SceneAlmostReady();
        public static event SceneAlmostReady OnSceneAlmostReady;

        public delegate void SceneLoaded();
        public static event SceneLoaded OnSceneLoad;

        public delegate void SceneUnloaded();
        public static event SceneUnloaded OnSceneUnload;


        [Header("Controls")]

        [SerializeField] bool _LoadFirstScene = false;
        public string _FirstSceneName = "";

        [SerializeField] bool _Debug = false;

        AsyncOperation asyncOperation = null;



        string lastSceneStarted = "";


        void Awake() {
            if (instance == null) {
                instance = this;
            } else if (instance != this) {
                Destroy(gameObject);
            }

            // Load the intro scene
            if (_LoadFirstScene || !Application.isEditor) {
                StartCoroutine(LoadingFirstScene());
            }
        }

        IEnumerator LoadingFirstScene() {
            if (_Debug) Debug.Log("[SceneUtilities] Loading the first scene: " + _FirstSceneName);

            BeginLoadScene(_FirstSceneName);
            yield return new WaitUntil(() => lastSceneStarted == _FirstSceneName);

            // Wait a sec then finish loading the intro scene
            yield return new WaitForSeconds(0.5f);
            FinishLoadScene(true);

        }



        public bool CheckSceneLoaded( string sceneName ) {
            if (SceneManager.GetSceneByName(sceneName).isLoaded) {
                if (_Debug) Debug.Log("[SceneUtilities] " + sceneName + " is loaded!");
                return true;
            }
            return false;
        }


        public bool CheckSceneActive( string sceneName ) {
            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName(sceneName) && DynamicGI.isConverged) {
                if (_Debug) Debug.Log("[SceneUtilities] " + sceneName + " is active!");
                return true;
            }
            return false;
        }



        /// <summary>
        /// Get the progress on the current scene load
        /// </summary>
        public float GetSceneProgress {
            get {
                if (asyncOperation != null) {
                    return asyncOperation.progress;
                } else {
                    return 0;
                }
            }
        }









        public void BeginLoadScene( string sceneToLoad ) {
            if (_Debug) Debug.Log("[SceneUtilities] Attempting to begin loading scene '" + sceneToLoad + "'");
            StartCoroutine(BeginLoadingScene(sceneToLoad));
        }
        IEnumerator BeginLoadingScene( string sceneToLoad ) {

            // Begin to load the new scene
            asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
            asyncOperation.allowSceneActivation = false;
            if (_Debug) Debug.Log("[SceneUtilities] Waiting for scene '" + sceneToLoad + "' to load...");

            // Wait until the new scene is almost loaded
            yield return new WaitUntil(() => asyncOperation.progress >= 0.9f);
            lastSceneStarted = sceneToLoad;

            // Send an event out for the caller script to pick up
            if (OnSceneAlmostReady != null) {
                if (_Debug) Debug.Log("[SceneUtilities] OnSceneAlmostReady event called");
                OnSceneAlmostReady();
            }

        }

        public void FinishLoadScene( bool setSceneActive ) {
            if (_Debug) Debug.Log("[SceneUtilities] Attempting to finish loading '" + lastSceneStarted + "'");
            StartCoroutine(FinishLoadingScene(setSceneActive));
        }
        IEnumerator FinishLoadingScene( bool setSceneActive ) {

            // Finish loading the new scene
            asyncOperation.allowSceneActivation = true;
            while (!asyncOperation.isDone) {
                yield return null;
            }

            // Set the new scene as active
            if (setSceneActive) {
                Debug.Log("[SceneUtilities] Attempting to set '" + lastSceneStarted + "' as active");

                while (SceneManager.GetActiveScene().name != lastSceneStarted) {
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName(lastSceneStarted));
                    yield return new WaitForSeconds(0.3f);
                }


                //yield return new WaitUntil(() => CheckSceneActive(lastSceneStarted));
                Debug.Log("[SceneUtilities] Set '" + lastSceneStarted + "' as active!");

            }

            // Wait until the dynamic GI is converged
            if (_Debug) Debug.Log("[SceneUtilities] Finished loading '" + lastSceneStarted + "'! Waiting for dynamic GI to update");
            yield return new WaitUntil(() => DynamicGI.isConverged);

            // Send an event out for the caller script to pick up
            if (OnSceneLoad != null) {
                if (_Debug) Debug.Log("[SceneUtilities] OnSceneLoad event called");
                OnSceneLoad();
            }


        }






        public void UnloadScene( string scene ) {
            if (_Debug) Debug.Log("[SceneUtilities] Attempting to unload scene '" + scene + "' asynchronously");
            StartCoroutine(UnloadingScene(scene));
        }

        IEnumerator UnloadingScene( string scene ) {

            // Unload the scene asynchronously
            asyncOperation = SceneManager.UnloadSceneAsync(scene);
            yield return new WaitUntil(() => !SceneManager.GetSceneByName(scene).isLoaded);
            yield return null;

            // Double check there are the right number of scenes loaded
            if (SceneManager.sceneCount > 2) {
                if (_Debug) Debug.Log("[SceneUtilities] Too many scenes, waiting for scene cleanup to complete...");
                ForceSceneCleanup();
                yield return new WaitUntil(() => SceneManager.sceneCount <= 2);
                if (_Debug) Debug.Log("[SceneUtilities] Scenes cleanup complete!");
            }

            // Flush any unloaded assets out of memory
            if (_Debug) Debug.Log("[SceneUtilities] '" + scene + "' unloaded, flushing unused assets");
            ForceUnloadUnusedAssets();


            // Send an event out for the caller script to pick up
            if (OnSceneUnload != null) {
                if (_Debug) Debug.Log("[SceneUtilities] OnSceneUnload event called");
                OnSceneUnload();
            }


        }



        public void ForceUnloadUnusedAssets() {
            StartCoroutine(FlushingUnusedAssets());
        }

        IEnumerator FlushingUnusedAssets() {

            if (_Debug) Debug.Log("[SceneUtilities] Flushing unused assets from memory");

            // Flush any unloaded assets out of memory
            asyncOperation = Resources.UnloadUnusedAssets();
            while (!asyncOperation.isDone) {
                yield return null;
            }

            if (_Debug) Debug.Log("[SceneUtilities] Unused assets flushed!");

        }




        void ForceSceneCleanup() {

            // Only the current scene and the ManagerScene are loaded
            if (SceneManager.sceneCount <= 2) return;

            // Check which scenes have to be removed
            bool currentSceneFound = false;
            for (int i = 0; i < SceneManager.sceneCount; i++) {
                Scene scene = SceneManager.GetSceneAt(i);

                // Skip the ManagerScene
                if (scene.name == "ManagerScene") continue;

                // Skip the current scene if it's the first one found
                if (scene.name == lastSceneStarted && !currentSceneFound) {
                    currentSceneFound = true;
                    continue;
                }

                // Otherwise, unload the scene
                SceneManager.UnloadSceneAsync(scene);
                
            }
        }
    }

}


