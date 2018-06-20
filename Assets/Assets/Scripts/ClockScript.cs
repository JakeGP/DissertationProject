using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClockScript : MonoBehaviour {

    Text hour, colon, minute;
    ThemeManager manager;
    GameManager gameManager;
    Interactable state;
    AudioSource source;
    public AudioClip off;

    float timer;
    float alarmTimer;

    bool clockToggle = false;

	// Use this for initialization
	void Start () {
        hour = transform.Find("Clockface").Find("Hour").gameObject.GetComponent<Text>();
        colon = transform.Find("Clockface").Find("Colon").gameObject.GetComponent<Text>();
        minute = transform.Find("Clockface").Find("Minute").gameObject.GetComponent<Text>();
        manager = GameObject.Find("GameManager").GetComponent<ThemeManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        state = GetComponent<Interactable>();
        source = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        hour.text = AddZeroIfNeeded(Mathf.RoundToInt(manager.timeOfDay));

        if (manager.timeOfDay != (int)manager.timeOfDay)
        {
            try
            {
                minute.text = AddZeroIfNeeded((float.Parse(manager.timeOfDay.ToString().Split('.')[1].Substring(0, 2)) / 100) * 60);
            }
            catch
            {
                minute.text = AddZeroIfNeeded((float.Parse(manager.timeOfDay.ToString()) / 100) * 60);
            }
        }
        else
        {
            minute.text = "00";
        }

        TickClock();

        if(!state.ObjectState)
        {
            if (!clockToggle)
            {
                gameManager.currentWaypointID = 0;
                clockToggle = true;
            }
            hour.color = new Color32(204, 255, 169, 255);
            minute.color = new Color32(204, 255, 169, 255);
            source.clip = off;
            source.loop = false;
        }

	}

    void TickClock()
    {
        timer += Time.deltaTime;
        alarmTimer += Time.deltaTime;

        hour.color = new Color32(204, 255, 169, 255);
        minute.color = new Color32(204, 255, 169, 255);

        if (alarmTimer > 0.5f)
        {
            if (state.ObjectState)
            {
                hour.color = new Color32(204, 255, 169, 0);
                minute.color = new Color32(204, 255, 169, 0);
            }
        }
        if(alarmTimer > 1.0f)
        {
            alarmTimer = 0.0f;
        }

        if (timer > 1.0f)
        {
            colon.text = "";

            if(timer > 2.0f)
            {
                colon.text = ":";
                timer = 0.0f;
            }
        }
    }


    string AddZeroIfNeeded(float number)
    {
        number = (int)number;
        if (number < 10)
        {
            return "0" + number.ToString();
        }
        else
        {
            return number.ToString();
        }
    }
}
