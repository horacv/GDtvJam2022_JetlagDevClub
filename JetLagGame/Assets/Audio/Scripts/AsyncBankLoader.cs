//--------------------------------------------------------------------
//
// This is a Unity behaviour script that demonstrates how to build
// a loading screen that can be used at the start of a game to
// load both FMOD Studio data and Unity data without blocking the
// main thread.
//
// To make this work properly "Load All Event Data at Initialization"
// needs to be turned off in the FMOD Settings.
//
// This document assumes familiarity with Unity scripting. See
// https://unity3d.com/learn/tutorials/topics/scripting for resources
// on learning Unity scripting. 
//
//--------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

class AsyncBankLoader : MonoBehaviour
{
    [FMODUnity.BankRef]
    public string bank;
    
    private float startTime;
    public float minimumLoadingTime;
    
    public string sceneToLoadNext = null;

    public void Start()
    {
        StartCoroutine(LoadGameAsync());
    }
    
    IEnumerator LoadGameAsync()
    {
        startTime = Time.time;
        
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneToLoadNext);
        
        async.allowSceneActivation = false;

        FMODUnity.RuntimeManager.LoadBank(bank, true);
        
        while (FMODUnity.RuntimeManager.AnySampleDataLoading())
        {
            yield return null;
        }

        while (Time.time < startTime + minimumLoadingTime)
        {
            yield return null;
        }
        
        async.allowSceneActivation = true;
        
        while (!async.isDone)
        {
            yield return null;
        }
    }
}
