using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;
using TMPro;
public class Manager : MonoBehaviour
{
    //Singleton pattern
    public static Manager instance;
    //General variables
    private WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
    public GameObject player;
    //UI elements
    public GameObject blackBox;
    public GameObject barkBox;
    public GameObject dialogueBox;
    private CanvasGroup blackBoxCG;
    public TextMeshProUGUI speakerDialogue;
    //Variables to hold coroutines
    private IEnumerator fadeInCR;
    private IEnumerator fadeOutCR;
    //Ink dialogue variables
    private Story currentStory;
    [Header("Ink json")]
    [SerializeField] public TextAsset inkJSON;
    private string convoName;

    private void Awake()
    {
        //make sure that there is only single instance of manager
        if (instance != null)
        {
            Debug.LogWarning("Found more than one manager");
        }
        instance = this;
    }

    void Start()
    {
        //set variables related to fading in and out
        blackBoxCG = blackBox.GetComponent<CanvasGroup>();
        blackBoxCG.alpha = 1;
        fadeOutCR = FadeOut();
        //set dialogue variables
        convoName = "Intro";
        currentStory = new Story(inkJSON.text);
        currentStory.ChoosePathString(convoName);
        ContinueStory();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            speakerDialogue.text = currentStory.Continue();
        }
    }

    //Called when player presses ok on black screen
    public void StartLoop() {
        StopCoroutine(fadeOutCR);
        fadeInCR = FadeIn();
        StartCoroutine(fadeInCR);
    }

    public void PlayerDeath(int n) {
        StopCoroutine(fadeInCR);
        //load next bark
        convoName=String.Concat("Bark", n-1);
        currentStory.ChoosePathString(convoName);
        ContinueStory();
        //fade to black
        fadeOutCR = FadeOut();
        StartCoroutine(fadeOutCR);
    }

    //Coroutines that fade scene in and out
    private IEnumerator FadeIn()
    {
        barkBox.SetActive(false);
        while (blackBoxCG.alpha > 0)
        {
            yield return waitForFixedUpdate;
            blackBoxCG.alpha -= 0.5f * Time.deltaTime;
        }
    }

    //coroutine that fades scene out
    private IEnumerator FadeOut()
    {
        while (blackBoxCG.alpha < 1)
        {
            yield return waitForFixedUpdate;
            blackBoxCG.alpha += 0.5f * Time.deltaTime;
        }
        player.GetComponent<Player>().ResetPlayer();
        barkBox.SetActive(true);
    }
}
