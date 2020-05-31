using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Paperticket;

public class CrossSceneEventHelper : MonoBehaviour
{
    
    // SCENE EVENTS
    public void LoadNextScene (string sceneName, float invokeTime ) {
        StartCoroutine(WaitThenLoadNextScene(sceneName, invokeTime));
    }
    IEnumerator WaitThenLoadNextScene( string sceneName, float invokeTime ) {
        yield return new WaitForSeconds(invokeTime);
        LoadNextScene(sceneName);
    }


    public void LoadNextScene( string sceneName ) {
        SceneUtilities.instance.BeginLoadScene(sceneName);
        SceneUtilities.OnSceneAlmostReady += LoadSceneCallback;

    }
    void LoadSceneCallback() {

        SceneUtilities.OnSceneAlmostReady -= LoadSceneCallback;
        SceneUtilities.instance.FinishLoadScene(true);
        SceneUtilities.instance.UnloadScene(gameObject.scene.name);

    }



    // PLAYER EVENTS
    
    public bool PlayerUsesGravity {
        get { return PTUtilities.instance.PlayerUsesGravity; }
        set { PTUtilities.instance.PlayerUsesGravity = value; }
    }

    public bool PlayerCanMove {
        get { return PTUtilities.instance.PlayerCanMove; }
        set { PTUtilities.instance.PlayerCanMove = value; }
    }


    // GENERAL EVENTS

    public void DestroyGameObject (GameObject objectToDestroy ) {
        Destroy(objectToDestroy);
    }
    


}
