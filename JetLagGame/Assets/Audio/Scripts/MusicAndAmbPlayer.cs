using System;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MusicAndAmbPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioManager audioManager;
    private FMOD.Studio.EventInstance musicEventInstance;
    private FMOD.Studio.EventInstance ambianceEventInstance;
    private static MusicAndAmbPlayer instance;

    void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        
        SetLevelState();
        
        PlayMusic(); 
        PlayAmbiance();
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
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(ambianceEventInstance, GetComponent<Transform>());
        ambianceEventInstance.start();
        ambianceEventInstance.release();
    }
    
    void SetLevelState()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "StartMenu":
                FMODUnity.RuntimeManager.StudioSystem.setParameterByNameWithLabel(audioManager.parameters.levelParameter,audioManager.parameters.levelParameterLabels[0]);
                break;
            case "blockout_01":
                FMODUnity.RuntimeManager.StudioSystem.setParameterByNameWithLabel(audioManager.parameters.levelParameter,audioManager.parameters.levelParameterLabels[1]);
                break;
        }
    }

    public void SetMenuAudio()
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByNameWithLabel(audioManager.parameters.levelParameter,audioManager.parameters.levelParameterLabels[0]);
    }
    public void SetLevel1Audio()
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByNameWithLabel(audioManager.parameters.levelParameter,audioManager.parameters.levelParameterLabels[1]);
    }
    public void SetWinAudio()
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByNameWithLabel(audioManager.parameters.levelParameter,audioManager.parameters.levelParameterLabels[2]);
    }
    
    public void SetLoseAudio()
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByNameWithLabel(audioManager.parameters.levelParameter,audioManager.parameters.levelParameterLabels[3]);
    }
}
