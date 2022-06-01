using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;
using System;

[System.Serializable]
public class ScoreSave
{
    public List<int> scores;
    public List<string> names;

    public ScoreSave(List<int> scores, List<string> names)
    {
        this.scores = scores;
        this.names = names;
    }
}

public class ListSorter : IComparer
{

    // Calls CaseInsensitiveComparer.Compare on the monster name string.
    int IComparer.Compare(System.Object x, System.Object y)
    {
        return ((new CaseInsensitiveComparer()).Compare(((GameObject)x).name, ((GameObject)y).name));
    }

}

public class ScoreManager : MonoBehaviour
{
    //this game object is never destroyed
    //it locates necessary game objects w/ tags when necessary 
    public static ScoreManager instance;
    
    //values
    public List<int> scores = new List<int>();
    public List<string> names = new List<string>();
    public List<int> maxScores = new List<int>();
    public List<int> oldIndices = new List<int>();

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

            LoadScores();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1 && scores.Count != 0)
        {
            //sort UI elements
            //Load UI elements that hold scores
            IComparer myComparer = new ListSorter();
            scoreTexts = GameObject.FindGameObjectsWithTag("Score");
            nameTexts = GameObject.FindGameObjectsWithTag("Name");
            Array.Sort(scoreTexts, myComparer);
            Array.Sort(nameTexts, myComparer);
            SortList();
            for (int i = 0; i < 5; i++)
            {
                if (scores.Count > i)
                {
                    nameTexts[i].GetComponent<TextMeshProUGUI>().text = names[oldIndices[i]];
                    scoreTexts[i].GetComponent<TextMeshProUGUI>().text = "" + maxScores[i];
                }
            }
        }
    }

    public void AddScore(int score, string name)
    {
        scores.Add(score);
        names.Add(name);

        SortList();

        SaveScores();
    }

    void SortList()
    {
        maxScores = new List<int>();
        oldIndices = new List<int>();
        int maxIndex = -1;
        for (int j = 0; j < scores.Count; j++)
        {
            //loop through scores
            int max = 0;
            //find max value and store location on scores list
            for (int i = 0; i < scores.Count; i++)
            {
                if (scores[i] > max && !oldIndices.Contains(i))
                {
                    max = scores[i];
                    maxIndex = i;
                }
            }
            //add the max value to max list
            maxScores.Add(max);
            //add the max location to index list
            //this will be used to locate the player's name from the names list
            oldIndices.Add(maxIndex);
        }//repeat for the number of elements in list
    }

    void SaveScores()
    {
        ScoreSave scoreSave = new ScoreSave(scores, names);

        string saveString = JsonUtility.ToJson(scoreSave);

        File.WriteAllText(Application.persistentDataPath + "/scores", saveString);
    }

    void LoadScores()
    {
       
        if (!File.Exists(Application.persistentDataPath + "/scores"))
            return;

        string saveString = File.ReadAllText(Application.persistentDataPath + "/scores");

        ScoreSave scoreSave = JsonUtility.FromJson<ScoreSave>(saveString);

        scores = scoreSave.scores;
        names = scoreSave.names;
    }
}
