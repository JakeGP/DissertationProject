using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts.Classes;
using UnityEngine.UI;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour {

    public GameObject[] waypoints;
    GameObject PlayerUI;

    public int startingWaypoint;
    public int currentWaypointID;
    public int lastWaypointID;
    public int waypointCheck;

    public bool isInside;

    public string currentObjective;

    public AudioMixer mixer;
    AsyncOperation async;

    bool asyncDone = false;
    float continueTimer = 0.0f;

    void Start() {
        ResetMixer();

        PlayerUI = GameObject.Find("PlayerUI");
        waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        mixer.SetFloat("MasterVolume", 0.0f);
        //Array.Reverse(waypoints);

        currentWaypointID = startingWaypoint;
        waypointCheck = currentWaypointID;
        lastWaypointID = currentWaypointID;
        if (currentWaypointID > 0)
        {
            currentObjective = waypoints.Length > 0 ? waypoints[currentWaypointID].GetComponent<WaypointScript>().objective : null;
        }

        waypoints = SortWaypoints(waypoints);

        if (SceneManager.GetActiveScene().name != "Menu")
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }

        if (GameObject.Find("ProgressManager") && !GameObject.Find("ProgressManager").GetComponent<ProgressManager>().debugMode && GameObject.Find("Framerate"))
            GameObject.Find("Framerate").GetComponent<Text>().enabled = false;
    }

    // Update is called once per frame
    void Update() {
        ToggleWaypoints();
        CheckAsync();
	}

    void ToggleWaypoints()
    {
        if (waypoints.Length > 0 && currentWaypointID >= 0 && currentWaypointID < waypoints.Length)
        {
            if (waypoints[currentWaypointID].GetComponent<WaypointScript>().objectiveMet)
            {
                waypoints[currentWaypointID].GetComponent<WaypointScript>().ToggleState();

                int nextWaypoint = waypoints[currentWaypointID].GetComponent<WaypointScript>().nextWaypointID;
                if (nextWaypoint != -1 && nextWaypoint != 999)
                {
                    waypoints[nextWaypoint].GetComponent<WaypointScript>().ToggleState();
                    currentObjective = waypoints[nextWaypoint].GetComponent<WaypointScript>().objective;
                    lastWaypointID = currentWaypointID;
                    currentWaypointID = nextWaypoint;
                    waypointCheck = currentWaypointID;
                }
                else if(nextWaypoint == -1)
                {
                    lastWaypointID = currentWaypointID;
                    waypointCheck = currentWaypointID;
                }
                else if(nextWaypoint == 999)
                {
                    GameObject.Find("Player").GetComponent<PlayerBehavior>().PlayerState = 999;
                    currentWaypointID = nextWaypoint;
                }
                
            }
            else if(waypointCheck != currentWaypointID && currentWaypointID >= 0)
            {
                if(lastWaypointID >= 0)
                {
                    foreach (GameObject waypoint in waypoints)
                    {
                        if (waypoint.GetComponent<WaypointScript>().isActive)
                        {
                            waypoint.GetComponent<WaypointScript>().isActive = false;
                            waypoint.GetComponent<WaypointScript>().HideWaypoint();
                        }
                    }
                }
                waypoints[currentWaypointID].GetComponent<WaypointScript>().isActive = true;
                currentObjective = waypoints[currentWaypointID].GetComponent<WaypointScript>().objective;
                lastWaypointID = currentWaypointID;
                waypointCheck = currentWaypointID;
            }
        }
    }


    GameObject[] SortWaypoints(GameObject[] waypoints)
    {
        GameObject[] sorted = waypoints.ToArray<GameObject>().OrderBy(i => i.GetComponent<WaypointScript>().waypointID).ToArray<GameObject>();

        return sorted;
    }

    public IEnumerator ASyncLoadGame()
    {
        ProgressManager progressManager = GameObject.Find("ProgressManager").GetComponent<ProgressManager>();
        progressManager.levelsPlayed.Add(progressManager.currentLevel);
        string levelToLoad = progressManager.RandomLevel();   
        progressManager.currentLevel = levelToLoad;

        async = SceneManager.LoadSceneAsync(levelToLoad);
        async.allowSceneActivation = false;

        while (!async.isDone)
        {
            PlayerUI.transform.Find("LoadingInstructions").Find("LoadingPercentage").Find("Percentage").GetComponent<Text>().text = "Loading.. " + ((int)(Mathf.Clamp01(async.progress / 0.9f) * 100)).ToString() + "%";
            yield return null;
            if (((async.progress / 0.9f) * 100) >= 95f)
            {
                asyncDone = true;
            }
        }
    }

    void CheckAsync()
    {
        if (asyncDone)
        {
            continueTimer += Time.deltaTime;

            if (continueTimer >= 10.0f)
            {
                PlayerUI.transform.Find("LoadingInstructions").Find("Continue").GetComponent<Text>().text = "Press any key to continue...";
                if (Input.anyKey)
                {
                    async.allowSceneActivation = true;
                }
            }
            if ((int)continueTimer % 2 == 0)
            {
                PlayerUI.transform.Find("LoadingInstructions").Find("Continue").GetComponent<Text>().text = "";
            }
            else
            {
                PlayerUI.transform.Find("LoadingInstructions").Find("Continue").GetComponent<Text>().text = "Press any key to continue...";
            }

        }
    }

    void ResetMixer()
    {
        mixer.SetFloat("AmbientSoundVol", -19.0f);
        mixer.SetFloat("AmbientSoundLowPass", 6000.0f);

        mixer.SetFloat("InsideSoundVol", -10.0f);
        mixer.SetFloat("InsideSoundLowPass", 22000.0f);
    }
}
