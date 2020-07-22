using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NarrativeClip {

    public string name;
    public AudioClip clip;

    public NarrativeClip (string clipName, AudioClip audioClip ) {
        name = clipName;
        clip = audioClip;
    }

}
