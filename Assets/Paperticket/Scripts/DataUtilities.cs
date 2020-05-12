using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

namespace Paperticket {

    public class DataUtilities : MonoBehaviour {
        public static DataUtilities instance = null;

        // RONE obb expansion file was "main.1.com.StudioBento.RONE.obb";

        [SerializeField] string _ExpansionFileName = null;
        [Tooltip("WARNING - > Make sure this matches the Bundle Version in Player Settings for every new build!")]
        [SerializeField] int _bundleIdentifier = 1;

        public string ExpansionFilePath {
            get {

                if (Application.platform == RuntimePlatform.WindowsEditor) {
                    return "D:/Paperticket Studios/PROJECTS/BUILDS/CarePlaysVR/EXEs/Asset Bundles/" + _ExpansionFileName;

                } else if (Application.platform == RuntimePlatform.WindowsPlayer) {
                    return Application.dataPath + "/Asset Bundles/" + _ExpansionFileName;

                } else if (Application.platform == RuntimePlatform.Android) {
                    return "/sdcard/Android/obb/" + Application.identifier + "/" + _ExpansionFileName;

                } else {
                    Debug.LogError("[DataUtilities] Bad platform for ExpansionFilePath!");
                    return "";
                }
            }
        }

        public string ExpansionFileName {
            get {

                if (Application.platform == RuntimePlatform.WindowsEditor) {
                    return "main." + _bundleIdentifier + ".com.WorkForcePlus.CarePlaysVR" + ".obb";

                } else if (Application.platform == RuntimePlatform.WindowsPlayer) {
                    return "main." + _bundleIdentifier + ".com.WorkForcePlus.CarePlaysVR" + ".obb";

                } else if (Application.platform == RuntimePlatform.Android) {
                    return "main." + _bundleIdentifier + "." + Application.identifier + ".obb";

                } else {
                    Debug.LogError("[DataUtilities] Bad platform for ExpansionFileName!");
                    return "";
                }
            }
        }

        public AssetBundle _ExpansionAssetBundle;



        [System.Serializable]
        class ProgressFloat {
            public string String;
            public float Float;
        }

        [SerializeField] bool _Debug;


        [Header("Video Data")]

        [SerializeField] string[] _VideoNames;


        [Header("Preloaded Progress Data")]

        public bool loadProgressOnStart;

        [SerializeField] List<string> StringKeys;
        [SerializeField] List<ProgressFloat> FloatKeys;



        void Awake() {

            // Set this object as the DataUtilities instance
            if (instance == null) {
                instance = this;
            } else if (instance != this) {
                Destroy(gameObject);
            }

            // Load the player's progress
            if (loadProgressOnStart) LoadPlayerProgress();

            // Load and save a reference to the expansion asset bundle containing the videos        
            //_ExpansionFileName = "main." + _bundleIdentifier + "." + Application.identifier + ".obb";
            _ExpansionFileName = ExpansionFileName;
            if (_Debug) Debug.Log("[DataUtilities] Attempting to grab ExpansionAssetBundle from: " + ExpansionFilePath);

            _ExpansionAssetBundle = AssetBundle.LoadFromFile(ExpansionFilePath);
            if (_Debug) Debug.Log("[DataUtilities]" + _ExpansionAssetBundle == null ? " Failed to load ExpansionAssetBundle" : " ExpansionAssetBundle successfully loaded!");

        }

        void LoadPlayerProgress() {

            // Load all string progress keys
            for (int i = 0; i < StringKeys.Count; i++) {
                SetProgressKey(StringKeys[i]);
                if (_Debug) Debug.Log("[DataUtilities] New string progress key added: " + StringKeys[i] + ", " + CheckProgressKey(StringKeys[i]));
            }

            // Load all float progress keys
            for (int i = 0; i < FloatKeys.Count; i++) {
                SetProgressKeyAsFloat(FloatKeys[i].String, FloatKeys[i].Float);
                if (_Debug) Debug.Log("[DataUtilities] New float progress key added: " + FloatKeys[i].String + ", value = " + GetProgressKeyAsFloat(FloatKeys[i].String));
            }

        }







        public bool CheckProgressKey( string keyName ) {
            if (PlayerPrefs.HasKey(keyName)) {
                return PlayerPrefs.GetInt(keyName) == 1;
            }
            if (_Debug) Debug.Log("[DataUtilities] Cannot find progress key '" + keyName + "' (string)");
            return false;
        }

        public float GetProgressKeyAsFloat( string keyName ) {
            if (PlayerPrefs.HasKey(keyName)) {
                return PlayerPrefs.GetFloat(keyName);
            }
            if (_Debug) Debug.Log("[DataUtilities] Cannot find progress key '" + keyName + "' (float)");
            return 0;
        }




        public void SetProgressKey( string keyName ) {
            PlayerPrefs.SetInt(keyName, 1);
            PlayerPrefs.Save();
        }

        public void SetProgressKeyAsFloat( string keyName, float value ) {
            PlayerPrefs.SetFloat(keyName, value);
            PlayerPrefs.Save();
        }



        public void ResetProgressKey( string keyName ) {
            if (PlayerPrefs.HasKey(keyName)) {
                PlayerPrefs.SetInt(keyName, 0);
                PlayerPrefs.Save();
            }
        }


        public void ClearPlayerProgress() {
            if (_Debug) Debug.Log("[DataUtilities] Clearing all player progress! OwO");

            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();

            // Load the player's progress
            if (loadProgressOnStart) {
                LoadPlayerProgress();
            }

        }



    }

}









//// Unless we're on PC, make sure all the videos are transfered to the persistant datapath
////if (Application.platform != RuntimePlatform.WindowsPlayer) {
////    StartCoroutine(CheckVideoStatus());
////}
//// NOT USED ATM
//IEnumerator CheckVideoStatus() {
//    if (_Debug) Debug.Log("[DataUtilities] Checking status of video files...");

//    string persistentPath = Path.Combine(Application.persistentDataPath, "Videos");

//    // Make sure that the persistant data path exists
//    if (!Directory.Exists(persistentPath)) {
//        if (_Debug) Debug.Log("[DataUtilities] Path '" + persistentPath + "' missing, creating now.");
//        Directory.CreateDirectory(persistentPath);
//    } else {
//        if (_Debug) Debug.Log("[DataUtilities] Path '" + persistentPath + "' found!");
//    }

//    // Copy each of the video files in turn
//    for (int i = 0; i < _VideoNames.Length; i++) {
//        string completeStreamingPath = Path.Combine(Application.streamingAssetsPath, _VideoNames[i]) + ".mp4";
//        string completePersistantPath = Path.Combine(persistentPath, _VideoNames[i]) + ".mp4";


//        // Skip re-copying if the file already exists
//        if (File.Exists(completePersistantPath)) {
//            if (_Debug) Debug.Log("[DataUtilities] Video '" + _VideoNames[i] + "' already in persistant path, ignoring");
//            //File.Delete(completePersistantPath);
//        } else {

//            // Put in a request for the video content
//            UnityWebRequest www = UnityWebRequest.Get(completeStreamingPath);

//            // Create the destination file in the persistant data path
//            var downloadHandler = new DownloadHandlerFile(completePersistantPath);
//            downloadHandler.removeFileOnAbort = true;

//            www.downloadHandler = downloadHandler;

//            yield return www.SendWebRequest();

//            if (www.isNetworkError || www.isHttpError)
//                Debug.LogError("[DataUtilities] ERROR -> " + www.error);
//            else
//                if (_Debug) Debug.Log("[DataUtilities] Download saved to: " + completePersistantPath.Replace("/", "\\") + "\r\n" + www.error);

//            // Close the web request and remove any resources
//            downloadHandler.Dispose();
//            www.Dispose();

//        }

//    }

//    yield return null;

//    SceneUtilities.instance.ForceUnloadUnusedAssets();

//}
