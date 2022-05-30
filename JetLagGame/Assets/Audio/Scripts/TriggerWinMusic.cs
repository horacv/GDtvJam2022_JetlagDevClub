using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerWinMusic : MonoBehaviour
{
    public MusicAndAmbPlayer musicAndAmbPlayer;
    
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        musicAndAmbPlayer.SetWinAudio();
    }
}
