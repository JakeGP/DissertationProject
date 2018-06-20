using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

    public Text percentage;
    public Dropdown levelSelect;
    public Dropdown debugDropdown;

    public GameObject m_progressManager;
    ProgressManager progressManager;

    bool loading = false;
    bool asyncDone = false;

    string levelToLoad;

    AsyncOperation async;

    public GameObject loadingText;

    float continueTimer = 0.0f;

    // Use this for initialization
    private void Awake()
    {
        InstantiateProgressManager();
    }

    void Start () {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        progressManager = GameObject.Find("ProgressManager").GetComponent<ProgressManager>();
        GameObject.Find("Reset").GetComponent<Button>().interactable = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (asyncDone)
        {
                continueTimer += Time.deltaTime;

            if (continueTimer >= 10.0f)
            {
                loadingText.transform.Find("Continue").GetComponent<Text>().text = "Press any key to continue...";
                if (Input.anyKey)
                {
                    async.allowSceneActivation = true;
                }

                if ((int)continueTimer % 2 == 0)
                {
                    loadingText.transform.Find("Continue").GetComponent<Text>().text = "";
                }
                else
                {
                    loadingText.transform.Find("Continue").GetComponent<Text>().text = "Press any key to continue...";
                }
            }
            
        }
	}

    public void PlayGame()
    {
        loadingText.SetActive(true);
        if (levelToLoad == "" || string.IsNullOrEmpty(levelToLoad))
        {
            levelToLoad = progressManager.RandomLevel();
        }

        if(levelToLoad == "Menu")
        {
            percentage.GetComponentInParent<Image>().color = new Color(0, 0, 0, 0.4f);
            percentage.text = "Complete";
            GameObject.Find("Reset").GetComponent<Button>().interactable = true;
        }
        else if (!loading)
        {
            loading = true;
            StartCoroutine(ASyncLoadGame(levelToLoad));
        }
        
    }

    public void OverrideLevelSelection()
    {
        levelToLoad = levelSelect.options[levelSelect.value].text;
    }

    IEnumerator ASyncLoadGame(string levelToLoad)
    {
        progressManager.currentLevel = levelToLoad;
        async = SceneManager.LoadSceneAsync(levelToLoad);
        async.allowSceneActivation = false;

        while (!async.isDone)
        {
            percentage.GetComponentInParent<Image>().color = new Color(0, 0, 0, 0.4f);
            percentage.text = "Loading.. " + ((int)(Mathf.Clamp01(async.progress / 0.9f) * 100)).ToString() + "%";
            yield return null;
            if(((async.progress / 0.9f) * 100) >= 95f)
            {
                asyncDone = true;
            }
        }
        
    }

    public void InstantiateProgressManager()
    {
        if(!GameObject.Find("ProgressManager"))
        {
            GameObject _progressManager = Instantiate(m_progressManager) as GameObject;
            _progressManager.name = "ProgressManager";
        }
    }

    public void ChangeDebugMode()
    {
        progressManager.debugMode = Convert.ToBoolean(debugDropdown.value);
    }

    public void Reset()
    {
        progressManager.levelsPlayed = new List<string> { };
        progressManager.currentLevel = "";
        percentage.GetComponentInParent<Image>().color = new Color(0, 0, 0, 0.0f);
        percentage.text = "";
        GameObject.Find("Reset").GetComponent<Button>().interactable = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
