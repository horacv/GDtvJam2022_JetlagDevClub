using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    [BankRef] public string masterBank;
    [BankRef] public string stringsBank;
    
    public AudioBusses audioBusses;
    public Parameters parameters;
    public SFX sfx;
    public Music music;

    private void Awake()
    {
        if (!RuntimeManager.HaveMasterBanksLoaded)
        {
            RuntimeManager.LoadBank(masterBank);
            RuntimeManager.LoadBank(stringsBank);
        }
    }
    
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

    public void PlayUICLick()
    {
        RuntimeManager.PlayOneShot(sfx.interactables.UI_Click);
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
        [Header("Level Parameters", order = 0)]
        public string levelParameter;
        public string[] levelParameterLabels;

        [Header("Moving Blocks", order = 1)]
        public string boxParameter;
        public string[] boxLabels;

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
            public EventReference scream;
        }

        [Serializable]
        public class Interactables
        {
            public EventReference movingBlocks;
            public EventReference fallingPlatforms;
            public EventReference UI_Click;
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
