using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotLights : MonoBehaviour {

    GameObject GameManager;
    ThemeManager Theme;

    Light light;

    public Color daytimeCol = new Color32(236, 197, 94, 255);
    public Color redskyCol = new Color32(176, 0, 18, 255);
    public Color nighttimeCol = new Color32(0, 21, 45, 255);

    public float daytimeLight = 28.0f;
    public float redskyLight = 0.28f;
    public float nighttimeLight = 0.15f;
    public float lerpAmount;
    float startingLight, currentLight;

    float lerpTime = 0.0f;
    float lightIntensity;

    float thunderTimer = 0.0f;
    bool flashThunder = false;

    // Use this for initialization
    void Start () {
        GameManager = GameObject.Find("GameManager");
        Theme = GameManager.GetComponent<ThemeManager>();

        light = GetComponent<Light>();
	}
	
	// Update is called once per frame
	void Update () {
        AutoTime();
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
        }
        else
        {
            thunderTimer += 1 * Time.deltaTime;
            light.intensity = 10.0f;
            light.color = Color.white;
            if (thunderTimer > 0.4f)
            {
                flashThunder = false;
            }
        }
    }

    public void FlashThunder()
    {
        flashThunder = true;
    }
}