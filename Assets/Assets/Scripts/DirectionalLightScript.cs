using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Classes;

public class DirectionalLightScript : MonoBehaviour {

    GameObject GameManager;
    ThemeManager Theme;

    Light light;

    public Color daytimeCol = new Color32(236, 197, 94, 255);
    public Color redskyCol = new Color32(176, 0, 18, 255);
    public Color nighttimeCol = new Color32(0, 21, 45, 255);
    Color startingColor, currentColor;

    public float daytimeLight = 2.10f;
    public float redskyLight = 0.28f;
    public float nighttimeLight = 0.15f;
    public float lerpAmount;
    float startingLight, currentLight;

    float lerpTime = 0.0f;
    float lightIntensity;

    float thunderTimer = 0.0f;
    bool flashThunder = false;

    LightState lastState = LightState.off;
    public LightState currentState = LightState.daylight;

    // Use this for initialization
    void Start () {
        GameManager = GameObject.Find("GameManager");
        Theme = GameManager.GetComponent<ThemeManager>();
        light = GetComponentInChildren<Light>();
        lastState = currentState;

        currentLight = light.intensity;
        currentColor = light.color;
    }
	
	// Update is called once per frame
	void Update () {
        AutoTime();
	}

    public void SetState()
    {
        if (lastState != currentState)
        {
            lerpTime = 0.0f;
            startingColor = currentColor;
            startingLight = currentLight;
        }

        switch (currentState)
        {

            case LightState.daylight:
                {
                    Daylight();
                    break;
                }
            case LightState.nighttime:
                {
                    Nighttime();
                    break;
                }
            case LightState.redsky:
                {
                    break;
                }
        }
        currentColor = light.color;
        currentLight = light.intensity;
        lastState = currentState;
        lerpTime += Time.deltaTime;
    }

    void Daylight()
    {
        light.intensity = Mathf.Lerp(startingLight, daytimeLight, lerpTime / 3.0f);
        light.color = Color.Lerp(startingColor, daytimeCol, lerpTime / 3.0f);
    }

    void Nighttime()
    {
        light.intensity = Mathf.Lerp(startingLight, nighttimeLight, lerpTime / 3.0f);
        light.color = Color.Lerp(startingColor, nighttimeCol, lerpTime / 3.0f);
    }

    void Redsky()
    {
        light.intensity = Mathf.Lerp(startingLight, nighttimeLight, lerpTime / 3.0f);
        light.color = Color.Lerp(startingColor, nighttimeCol, lerpTime / 3.0f);
    }

    void AutoTime()
    {
        if (!flashThunder)
        {
            float daytime = Theme.sunsetEnd - Theme.sunrise;

            if (Theme.isSunrise)
            {
                lerpAmount = (Theme.sunriseEnd - Theme.timeOfDay) / Theme.sunriseLength;
                light.intensity = Mathf.Lerp(daytimeLight, nighttimeLight, lerpAmount);
                light.color = Color.Lerp(daytimeCol, nighttimeCol, lerpAmount);
            }

            if (Theme.isSunset)
            {
                lerpAmount = (Theme.sunsetEnd - Theme.timeOfDay) / Theme.sunsetLength;
                light.intensity = Mathf.Lerp(nighttimeLight, daytimeLight, lerpAmount);
                light.color = Color.Lerp(nighttimeCol, daytimeCol, lerpAmount);
            }

            if (Theme.isDaytime)
            {
                light.intensity = daytimeLight;
                light.color = daytimeCol;
            }

            if (Theme.isNighttime)
            {
                light.intensity = nighttimeLight;
                light.color = nighttimeCol;
            }

            if (Theme.timeOfDay > Theme.sunrise && Theme.timeOfDay < Theme.sunsetEnd)
            {
                float percent = (Theme.timeOfDay - Theme.sunrise) / daytime;
                float angle = percent * 180;

                transform.rotation = Quaternion.Euler(angle, 130, 0);
            }
        }
        else
        {
            thunderTimer += 1 * Time.deltaTime;
            light.intensity = 14.0f;
            light.color = Color.white;
            if (thunderTimer > 0.5f)
            {
                thunderTimer = 0.0f;
                flashThunder = false;
            }
        }
    }

    public void FlashThunder()
    {
        flashThunder = true;
    }
}
