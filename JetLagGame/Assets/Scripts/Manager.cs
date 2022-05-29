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
    //UI elements
    public GameObject blackBox;
    private CanvasGroup blackBoxCG;
    private bool black = true;
    public TextMeshProUGUI speakerDialogue;

    //Coroutines
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
        fadeInCR = FadeIn();
        fadeOutCR = FadeOut();
        black = true;
        //set dialogue variables
        convoName = "Barks";
        currentStory = new Story(inkJSON.text);
        currentStory.ChoosePathString(convoName);
        ContinueStory();
    }

    // Update is called once per frame
    void Update()
    {
        if (!black){StopCoroutine(fadeInCR);}
    }

    public void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            speakerDialogue.text = currentStory.Continue();

        }
    }

    //Called at beginning/after death
    public void StartLoop() {
        StartCoroutine(fadeInCR);
    }

    //Coroutines that fade scene in and out
    private IEnumerator FadeIn()
    {
        while (blackBoxCG.alpha >= 0)
        {
            yield return waitForFixedUpdate;
            blackBoxCG.alpha -= 0.3f * Time.deltaTime;
        }
        black = false;
    }

    //coroutine that fades scene out
    private IEnumerator FadeOut()
    {
        while (blackBoxCG.alpha <= 1)
        {
            yield return waitForFixedUpdate;
            blackBoxCG.alpha += 0.3f * Time.deltaTime;
        }
        black = true;
    }
}
