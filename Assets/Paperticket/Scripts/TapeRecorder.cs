using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using TMPro;

public class TapeRecorder : MonoBehaviour {

    public static TapeRecorder instance;

    [SerializeField] bool debugging;

    [Header("References")]

    [SerializeField] AudioSource source;
    [SerializeField] TextMeshPro displayText;


    [Header("Controls")]
    [Space(10)]
    [SerializeField] bool autoPlay;
    [Space(5)]
    [SerializeField] [Range(0, 0.99f)] float indexTriggerDeadzone = 0.5f;
    [SerializeField] [Range(0, 0.99f)] float gripTriggerDeadzone = 0.5f;
    [Space(5)]
    [SerializeField] [Range(0, 2)] float skipMultiplier;

    [Header("Read Only")]
    [Space(10)]
    [SerializeField] bool playing;
    [SerializeField] float currentTime;
    //[SerializeField] List<AudioClip> clipQueue = new List<AudioClip>();
    [SerializeField] List<NarrativeClip> narrativeQueue = new List<NarrativeClip>();
    [SerializeField] int clipIndex;
         
    Coroutine playingCo;

    
    public bool isPlaying { get { return playing; } }
    public float time { get { return currentTime; } }
    //public float speed { get { return source.pitch; } set { source.pitch = value; } }


    bool indexReset = false;
    float speed = 1;
    void Update() {

        // Check face buttons for next/previous clip
        if (CrossPlatformInputManager.GetButtonDown("LeftXButton")) {
            Next();
        } else if (CrossPlatformInputManager.GetButtonDown("LeftYButton")) {
            Previous();
        }

        //Debug.Log("[TapeRecorder] index = " + CrossPlatformInputManager.GetAxis("LeftIndexTrigger"));

        // Check index trigger for play/pause narration
        if (CrossPlatformInputManager.GetAxis("LeftIndexTrigger") > indexTriggerDeadzone) {
            if (!indexReset) {
                if (playing) Pause();
                else Play();
                indexReset = true;
            }
        } else if (indexReset) indexReset = false;


        // Check grip trigger for skip speed
        if (playing) {
            speed = 1;            
            if (CrossPlatformInputManager.GetAxis("LeftGripTrigger") > gripTriggerDeadzone) {
                speed += skipMultiplier * Mathf.Clamp01((CrossPlatformInputManager.GetAxis("LeftGripTrigger")) / (1 - gripTriggerDeadzone));
            }
            source.pitch = speed;
        }
        

    }



    void Awake() {
        if (instance!=null) {Debug.LogError("[TapeRecorderer] ERROR -> instance already defined! disabling"); enabled=false;} 
        else instance=this;  
    }

    public void Play() {
        if (debugging) Debug.Log("[TapeRecorder] Beginning narration playback");
        playing = true;
        if (playingCo != null) StopCoroutine(playingCo);
        playingCo = StartCoroutine(PlayingClip());
    }

    public void Pause() {
        if (debugging) Debug.Log("[TapeRecorder] Pausing narration playback");
        playing = false;
        if (playingCo != null) StopCoroutine(playingCo);
        source.Pause();
    }

    public void Next() {
        if (clipIndex >= narrativeQueue.Count - 1) return;
        if (debugging) Debug.Log("[TapeRecorder] Going to next narration clip");
        if (playingCo != null) StopCoroutine(playingCo);

        source.Stop();
        currentTime = 0;
        clipIndex += 1;
        // Clamp the index to stop looping forward and back
        clipIndex = Mathf.Clamp(clipIndex, 0, narrativeQueue.Count - 1);
        displayText.text = narrativeQueue[clipIndex].name;

        if (playing) playingCo = StartCoroutine(PlayingClip());
    }

    public void Previous() {
        if (clipIndex == 0) return;
        if (debugging) Debug.Log("[TapeRecorder] Going to previous narration clip");
        if (playingCo != null) StopCoroutine(playingCo);

        source.Stop();
        currentTime = 0;
        clipIndex -= 1;
        // Modulo the index to keep it reasonable
        //clipIndex = clipIndex % clipQueue.Count;
        // Clamp the index to stop looping forward and back
        clipIndex = Mathf.Clamp(clipIndex, 0, narrativeQueue.Count - 1);
        displayText.text = narrativeQueue[clipIndex].name;

        if (playing) playingCo = StartCoroutine(PlayingClip());

    }

    public void PlayAtIndex (int index ) {
        if (index >= narrativeQueue.Count) {
            Debug.LogError("[TapeRecorder] ERROR -> Bad index (" + index + "), ignoring");
            return;
        }
        if (debugging) Debug.Log("[TapeRecorder] Playing narration clip at index (" + index + ")");
        if (playingCo != null) StopCoroutine(playingCo);

        source.Stop();
        currentTime = 0;
        clipIndex = index;
        displayText.text = narrativeQueue[clipIndex].name;

        playing = true;
        playingCo = StartCoroutine(PlayingClip());
    }



    IEnumerator PlayingClip() {
        if (debugging) Debug.Log("[TapeRecorder] Playing narration clip (" + narrativeQueue[clipIndex].name + "), queue index (" + clipIndex + ")");

        // Make sure the source is set and start playing
        if (source.clip != narrativeQueue[clipIndex].clip) source.clip = narrativeQueue[clipIndex].clip;
        source.Play();
        yield return new WaitForSeconds(0.1f);

        // Update the time until the clip finishes
        while (source.isPlaying) {
            currentTime = source.time;
            yield return null;            
        }
        yield return null;
        //source.Stop();        

        // Stop playing if autoplay is off or we just played the last clip
        if (!autoPlay || clipIndex >= narrativeQueue.Count - 1) playing = false;

        // Goto the next clip
        Next();
    }





    //public void AddNarrationClip( AudioClip clip ) {

    //    if (!clipQueue.Contains(clip)) {
    //        if (debugging) Debug.Log("[TapeRecorderer] Added narration audio (" + clip.name + ") to clip queue");
    //        clipQueue.Add(clip);
    //    } else Debug.LogWarning("[TapeRecorderer] Narration audio (" + clip.name + ") already in clip queue, disregarding");

    //    // Start playin the new clip
    //    PlayAtIndex(clipQueue.Count - 1);
    //}


    public void AddNarrationClip( NarrativeClip clip ) {

        if (!narrativeQueue.Contains(clip)) {
            if (debugging) Debug.Log("[TapeRecorderer] Added narration audio (" + clip.name + ") to clip queue");
            narrativeQueue.Add(clip);
        } else Debug.LogWarning("[TapeRecorderer] Narration audio (" + clip.name + ") already in clip queue, disregarding");
        
        // Start playin the new clip
        PlayAtIndex(narrativeQueue.Count - 1);
    }

}
