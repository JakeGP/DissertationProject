using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Classes;

public class Lampshade : MonoBehaviour {

    GameObject GameManager;
    ThemeManager theme;

    public float lerpTime;

    public LightState currentState = LightState.off, lastState;

    // Use this for initialization
    void Start () {
        GameManager = GameObject.Find("GameManager");
        theme = GameManager.GetComponent<ThemeManager>();
    }
	
	// Update is called once per frame
	void Update () {
        CheckTime();
        SetState();
	}

    void SetState()
    {
        if (lastState != currentState)
            lerpTime = 0.0f;

        switch (currentState)
        {

            case LightState.on:
                {
                    LightOn();
                    break;
                }
            case LightState.off:
                {
                    LightOff();
                    break;
                }
        }

        lastState = currentState;
    }

    void LightOn()
    {
        lerpTime += Time.deltaTime;
        GetComponent<MeshRenderer>().materials[0].SetColor("_EmissionColor", Color.HSVToRGB(36.0f/359.0f, 1.0f, Mathf.Lerp(0.0f, 1.0f, lerpTime / 1.0f)));
        GetComponent<MeshRenderer>().materials[0].EnableKeyword("_EMISSION");

    }

    void LightOff()
    {
        lerpTime += Time.deltaTime;
        GetComponent<MeshRenderer>().materials[0].SetColor("_EmissionColor", Color.HSVToRGB(36.0f/359.0f, 1.0f, Mathf.Lerp(1.0f, 0.0f, lerpTime / 1.0f)));
    }

    void CheckTime()
    {
        if (theme.isDaytime)
        {
            currentState = LightState.off;
        }
        else if (theme.isNighttime)
        {
            currentState = LightState.on;
        }
    }
}
