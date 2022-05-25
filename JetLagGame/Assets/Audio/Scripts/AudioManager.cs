using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public AudioBusses audioBusses;
    public Parameters parameters;
    public SFX sfx;
    public Music music;

    void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        
    }

    [Serializable]
    public class AudioBusses
    {
        public string masterBus;
        public string sfxBus;
        public string musicBus;
    }
    
    [Serializable]
    public class Parameters
    {
        [Header("Music Cues Parameters", order = 0)]
        public string musicParameter;
        public string[] musicParameterLabels;
    }
    
    [Serializable]
    public class  SFX
    {
        public MainCharacter mainCharacter;
        public Interactables interactables;
        public Ambiance ambiance;
        
        [Serializable]
        public class MainCharacter
        {
            public EventReference footsteps;
            public EventReference jump;
        }

        [Serializable]
        public class Interactables
        {
            public EventReference boxDrag;
        }
        
        [Serializable]
        public class Ambiance
        {
            public EventReference ambiance;
        }
    }

    [Serializable]
    public class Music
    {
        public EventReference music;
    }
}
