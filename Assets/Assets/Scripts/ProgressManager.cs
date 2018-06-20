using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ProgressManager : MonoBehaviour {

    public string currentLevel;
    public List<string> levels;
    public List<string> levelsPlayed;

    public bool debugMode = false;

    // Use this for initialization
    void Start () {
        levels = new List<string> { "Anger", "Fear", "Sadness", "Happiness" };
        DontDestroyOnLoad(this);
	}

    public string RandomLevel()
    {
        bool levelChosen = false;
        string randomLevel = "";

        if (levelsPlayed.Count != levels.Count)
        {
            while (!levelChosen)
            {
                randomLevel = levels[(int)UnityEngine.Random.Range(0, levels.Count)];
                levelChosen = true;
                foreach (string level in levelsPlayed)
                {
                    if (randomLevel == level)
                    {
                        levelChosen = false;
                    }
                }
            }
        }
        else
        {
            return "Menu";
        }

        return randomLevel;
    }


    // Update is called once per frame
    void Update () {
		
	}
}
