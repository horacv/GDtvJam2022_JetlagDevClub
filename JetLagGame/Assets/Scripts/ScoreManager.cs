using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    //this game object is never destroyed
    //it locates necessary game objects w/ tags when necessary 
    public static ScoreManager instance;
    
    //values
    public List<int> scores = new List<int>();
    public List<string> names = new List<string>();

    //UI elements that show those values
    public GameObject[] scoreTexts;
    public GameObject[] nameTexts;
    public GameObject startButton;
    //manager 
    public GameObject manager;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        scoreTexts = GameObject.FindGameObjectsWithTag("Score");
        nameTexts = GameObject.FindGameObjectsWithTag("Name");
        manager = GameObject.FindGameObjectWithTag("Manager");
        startButton= GameObject.FindGameObjectWithTag("StartButton");
        if (startButton != null) {
            startButton.GetComponent<Button>().onClick.AddListener(StartGame);
        }
        //sort scores
        if (SceneManager.GetActiveScene().buildIndex == 2 && scores.Count!=0) {
            nameTexts[0].GetComponent<TextMeshProUGUI>().text = "" + names[0];
            scoreTexts[0].GetComponent<TextMeshProUGUI>().text= ""+scores[0];
        }
    }

    public void StartGame() {
        SceneManager.LoadScene(1);
    }
}
