using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Paperticket;
using TMPro;

public class CrossSceneEventHelper : MonoBehaviour
{

    // SCENE EVENTS
    public void LoadNextScene( string sceneName, float invokeTime ) {
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

    public bool ToggleTapeRecorder {
        get { return PTUtilities.instance.ToggleTapeRecorder; }
        set { PTUtilities.instance.ToggleTapeRecorder = value; }

    }

    public bool ToggleFlashlight {
        get { return PTUtilities.instance.ToggleFlashlight; }
        set { PTUtilities.instance.ToggleFlashlight = value; }

    }

    public void FadeHeadToBlack( float duration ) {
        PTUtilities.instance.FadeHeadToBlack(duration);
    }

    public void FadeHeadToClear( float duration ) {
        PTUtilities.instance.FadeHeadToClear(duration);
    }






    // GENERAL EVENTS

    public void DestroyGameObject (GameObject objectToDestroy ) {
        Destroy(objectToDestroy);
    }
    
    public void FadeSpriteIn (SpriteRenderer sprite) {
        StartCoroutine(PTUtilities.instance.FadeAlphaTo(sprite, 1, 1.5f));
    }

    public void FadeSpriteOut( SpriteRenderer sprite ) {
        StartCoroutine(PTUtilities.instance.FadeAlphaTo(sprite, 0, 1.5f));
    }

    public void FadeTextIn( TextMeshPro text ) {
        StartCoroutine(PTUtilities.instance.FadeAlphaTo(text, 1, 1.5f));
    }

    public void FadeTextOut( TextMeshPro text ) {
        StartCoroutine(PTUtilities.instance.FadeAlphaTo(text, 0, 1.5f));
    }


    public void PlayAudioClip( AudioClip clip ) {

        AudioManager.instance.PlayAudioClip(clip, PTUtilities.instance.HeadsetPosition(), 1f);

    }

}
