using UnityEngine.SceneManagement;
using UnityEngine;

public class MusicAndAmbPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioManager audioManager;
    private FMOD.Studio.EventInstance musicEventInstance;
    private FMOD.Studio.EventInstance ambianceEventInstance;
    
    private const string level1Name = "blockout_01"; 
    
    void Start()
    {
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
            case level1Name:
                FMODUnity.RuntimeManager.StudioSystem.setParameterByNameWithLabel(audioManager.parameters.levelParameter,audioManager.parameters.levelParameterLabels[0]);
                break;
        }
    }
}
