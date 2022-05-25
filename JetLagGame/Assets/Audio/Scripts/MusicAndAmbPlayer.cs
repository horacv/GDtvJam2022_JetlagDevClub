using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicAndAmbPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioManager audioManager;
    private FMOD.Studio.EventInstance musicEventInstance;
    private FMOD.Studio.EventInstance ambianceEventInstance;
    void Start()
    {
        PlayMusic(); 
        //PlayAmbiance();
    }
    
    void PlayMusic()
    {
        musicEventInstance = FMODUnity.RuntimeManager.CreateInstance(audioManager.music.music);
        musicEventInstance.start();
        musicEventInstance.release();
    }

    void PlayAmbiance()
    {
        ambianceEventInstance = FMODUnity.RuntimeManager.CreateInstance(audioManager.sfx.ambiance.ambiance);
        ambianceEventInstance.start();
        ambianceEventInstance.release();
    }
}
