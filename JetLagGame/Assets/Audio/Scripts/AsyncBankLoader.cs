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
