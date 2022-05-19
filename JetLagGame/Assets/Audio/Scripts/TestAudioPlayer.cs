using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAudioPlayer : MonoBehaviour
{
    [SerializeField]
    private FMODUnity.EventReference testEvent;
    void Start()
    {
        FMODUnity.RuntimeManager.PlayOneShot(testEvent);
    }

    
}
