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
    public Light sceneLight;
    public Light spotLight;
    public Light winLight;
    public static int score;
    public static int winScore=500;
    public static bool ascending;
    //UI elements
    public GameObject blackBox;
    public GameObject barkBox;
    public GameObject dialogueBox;
    private CanvasGroup blackBoxCG;
    public TextMeshProUGUI speakerDialogue;//dialogue at center of screen
    public TextMeshProUGUI speakerDialogue2;//dialogue at bottom of screen
    public TextMeshProUGUI scoreText;
    private bool barkTriggered;//bark was triggered instead of being called automatically
    //Variables to hold coroutines
    private IEnumerator fadeInCR;
    private IEnumerator fadeOutCR;
    //Ink dialogue variables
    private Story currentStory;
    public TextAsset inkJSON;
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
        //set lighting
        sceneLight.enabled = true;
        spotLight.enabled = false;
        winLight.enabled = false;
        //set variables related to fading in and out
        blackBoxCG = blackBox.GetComponent<CanvasGroup>();
        blackBoxCG.alpha = 1;
        fadeOutCR = FadeOut();
        dialogueBox.SetActive(false);
        barkTriggered = false;
        //set dialogue variables
        convoName = "Intro";
        currentStory = new Story(inkJSON.text);
        currentStory.ChoosePathString(convoName);
        speakerDialogue.text = currentStory.Continue();
        //set game variables
        score = 1000;
        ascending = false;
    }

    void Update()
    {
        scoreText.text = "Score: " + score;
        if (ascending) {
            player.transform.position += new Vector3(0, 5*Time.deltaTime, 0);
        }
        if (player.transform.position.y > 20) {
            Application.Quit();
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }

    //Called when player presses ok on black screen
    public void StartLoop() {
        StopCoroutine(fadeOutCR);
        fadeInCR = FadeIn();
        StartCoroutine(fadeInCR);
    }

    public void PlayerDeath(int n) {
        if (score > 0) { 
            score = score - 100;
        }
        
        StopCoroutine(fadeInCR);
        //load next bark
        if (!barkTriggered) { convoName = String.Concat("Bark", n); }
        currentStory.ChoosePathString(convoName);
        speakerDialogue.text = currentStory.Continue();
        barkTriggered = false;
        //fade to black
        fadeOutCR = FadeOut();
        StartCoroutine(fadeOutCR);
    }

    public void DialogueTriggered(string s) {
        if (s == "Trigger1")
        {
            Player.canMove = false;
            currentStory.ChoosePathString(s);
            speakerDialogue2.text = currentStory.Continue();
            dialogueBox.SetActive(true);
        }
        else if (s == "Trigger2")
        {
            barkTriggered = true;
            convoName = s;
            score = score + 100;
            player.GetComponent<Player>().jumpForce = 15;
        }
        else if (s == "Trigger3")
        {
            barkTriggered = true;
            convoName = s;
            player.GetComponent<Player>().lightAngle = 90;
        }
        else if (s == "LightOn")
        {
            sceneLight.enabled = true;
            spotLight.enabled = false;
        }
        else if (s == "LightOff") {
            sceneLight.enabled = false;
            spotLight.enabled = true;
        }
        else if (s == "Trigger4")
        {
            Player.canMove = false;
            if (score >= winScore) {
                currentStory.ChoosePathString("Trigger4W");
                winLight.enabled = true;
            }
            else { currentStory.ChoosePathString("Trigger4L"); }
            speakerDialogue2.text = currentStory.Continue();
            dialogueBox.SetActive(true);
        }
    }

    public void DialogueClosed() {
        Player.canMove = true;
        dialogueBox.SetActive(false);
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
