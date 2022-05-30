using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuManager : MonoBehaviour
{
    public MusicAndAmbPlayer musicAndAmbPlayer;
    public void StartGame()
    {
        musicAndAmbPlayer.SetLevel1Audio();
        SceneManager.LoadScene(2);
    }
}
