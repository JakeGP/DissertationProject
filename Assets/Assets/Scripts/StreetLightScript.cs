using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Classes;

public class StreetLightScript : MonoBehaviour {

    public GameObject glass;
    Light light;
    ThemeManager theme;
    MeshRenderer glassMesh;

    float lerpTime = 0.0f;
    float lightIntensity;

    LightState lastState;
    public LightState overrideState = LightState.Default;
    public LightState currentState = LightState.off;

    // Use this for initialization
    void Start() {
        theme = GameObject.Find("GameManager").GetComponent<ThemeManager>();
        glassMesh = glass.GetComponent<MeshRenderer>();
        light = GetComponentInChildren<Light>();
        lastState = currentState;
    }

    // Update is called once per frame
    void Update() {
        CheckTime();
        SetState();
    }

    void SetState()
    {
        if (overrideState != LightState.Default)
        {
            currentState = overrideState;
        }
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
            case LightState.redsky: {
                    RedLightOn();
                    break;
                }
            case LightState.flicker:
                {
                    LightFlicker();
                    break;
                }
        }

        lastState = currentState;
    }

    void LightOn()
    {
        lerpTime += Time.deltaTime;
        light.intensity = Mathf.Lerp(0.0f, 15.0f, lerpTime / 1.0f);
        light.color = new Color32(171, 161, 108, 255);
        glassMesh.materials[0].SetColor("_EmissionColor", Color.HSVToRGB(0, 0, Mathf.Lerp(0.0f, 1.0f, lerpTime / 1.0f)));
        glassMesh.materials[0].EnableKeyword("_EMISSION");

    }

    void LightOff()
    {
        lerpTime += Time.deltaTime;
        light.color = new Color32(171, 161, 108, 255);
        glassMesh.materials[0].SetColor("_EmissionColor", Color.HSVToRGB(0, 0, Mathf.Lerp(1.0f, 0.0f, lerpTime / 1.0f)));
        light.intensity = Mathf.Lerp(15.0f, 0.0f, lerpTime / 1.0f);
    }

    void RedLightOn()
    {
        light.intensity = 15.0f;
        light.color = new Color(0.6911765f, 0.2134515f, 0.2134515f);
        glassMesh.materials[0].EnableKeyword("_EMISSION");
        glassMesh.materials[0].SetColor("_EmissionColor", new Color(0.6911765f, 0.2134515f, 0.2134515f));
    }

    void LightFlicker()
    {
        if(Random.Range(0.0f, 100.0f) > 80.0f)
        {
            float intensity = Random.Range(0.0f, 10.0f);
            light.intensity = intensity;
            light.color = new Color32(171, 161, 108, 255);
            glassMesh.materials[0].EnableKeyword("_EMISSION");
            glassMesh.materials[0].SetColor("_EmissionColor", Color.HSVToRGB(0, 0, intensity / 10));
        }
    }

    void CheckTime()
    {
        if(theme.isDaytime)
        {
            currentState = LightState.off;
        }
        else if(theme.isNighttime)
        {
            currentState = LightState.on;
        }
    }
}
