using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Classes;
using DigitalRuby.RainMaker;

public class ThemeManager : MonoBehaviour {

    [HideInInspector] public GameObject directionalLight;
    [HideInInspector] public GameObject rain;

    [HideInInspector] public Material skybox;

    [HideInInspector] GameObject[] streetLights;
    [HideInInspector] GameObject[] houseSpotlights;

    public AudioClip thunderCrack;

    public Theme currentTheme = Theme.Default;
    private Theme lastTheme = Theme.Off;

    public Color happiness_AmbientDayColor;
    public Color happiness_AmbientNightColor;
    public Color sadness_AmbientDayColor;
    public Color sadness_AmbientNightColor;
    public Color anger_AmbientDayColor;
    public Color anger_AmbientNightColor;
    public Color fear_AmbientDayColor;
    public Color fear_AmbientNightColor;
    public Color default_AmbientDayColor;
    public Color default_AmbientNightColor;
    private Color ambientDayColor;
    private Color ambientNightColor;

    public float sunrise = 6.0f;
    public float sunriseLength = 4.0f;
    [HideInInspector] public float sunriseEnd;
    public float sunset = 17.0f;
    public float sunsetLength = 4.0f;
    [HideInInspector] public float sunsetEnd;

    [HideInInspector] public bool isSunrise = false;
    [HideInInspector] public bool isSunset = false;
    [HideInInspector] public bool isDaytime = false;
    [HideInInspector] public bool isNighttime = false;

    public float timeOfDay;
    public float skyboxValue = 0.0f;
    public float rainIntensity = 0.0f;
    public float timeScale = 1.0f;
    private float thunderTimer = 0.0f;
    private float randomThunderVal = 2.0f;
    private float skyboxOverrideVal = 0.0f;
    private float blend1 = 0.0f;
    private float blend2 = 0.0f;

    public bool randomRain = false;
    public bool randomThunder = false;
    public bool thunder = false;
    public bool autoTime = false;
    private bool allowAutoTime = false;
    private bool rainToggle = false;
    private bool thunderToggle = false;
    private bool thunderFlash = false;
    private bool thunderReset = false;
    private bool skyboxOverride = false;

	// Use this for initialization
	void Start () {
        streetLights = GameObject.FindGameObjectsWithTag("Street Light");
        directionalLight = GameObject.Find("Directional Light");
        rain = GameObject.Find("RainPrefab");
        houseSpotlights = GameObject.FindGameObjectsWithTag("OutDoorSpotlights");
        skybox = RenderSettings.skybox;
    }
	
	// Update is called once per frame
	void Update () {
        SetTheme();
        TimeOfDay();
        ChangeSkybox();
        ChangeRain();
        Keyboard();
    }

    //Public Global Change Theme Function
    public void ChangeTheme(Theme _theme)
    {
        currentTheme = _theme;
    } 

    //Set Theme
    void SetTheme()
    {
        if (lastTheme != currentTheme)
        {
            switch (currentTheme)
            {
                case Theme.Default:
                    {
                        Default();
                        break;
                    }
                case Theme.Happiness:
                    {
                        Happiness(); ;
                        break;
                    }
                case Theme.Sadness:
                    {
                        Sadness();
                        break;
                    }
                case Theme.Anger:
                    {
                        Anger();
                        break;
                    }
                case Theme.Fear:
                    {
                        Fear();
                        break;
                    }
            }

            lastTheme = currentTheme;
        }
    }

    //Themes
    void Default()
    {
        ChangeStreetLights(LightState.Default);
        timeOfDay = 9.0f;
        thunder = false;
        rainIntensity = 0.0f;
        allowAutoTime = true;
        skyboxOverride = false;

        ambientDayColor = default_AmbientDayColor;
        ambientNightColor = default_AmbientNightColor;
    }
    void Happiness()
    {
        ChangeStreetLights(LightState.off);
        timeOfDay = 9.0f;
        thunder = false;
        rainIntensity = 0.0f;
        allowAutoTime = false;
        skyboxOverride = false;
        skyboxOverrideVal = 0.0f;

        ambientDayColor = happiness_AmbientDayColor;
        ambientNightColor = happiness_AmbientNightColor;

    }
    void Sadness()
    {
        ChangeStreetLights(LightState.on);
        timeOfDay = 18.0f;
        thunder = false;
        rainIntensity = 0.5f;
        allowAutoTime = false;
        skyboxOverride = false;
        skyboxOverrideVal = 0.0f;

        ambientDayColor = sadness_AmbientDayColor;
        ambientNightColor = sadness_AmbientNightColor;
    }
    void Fear()
    {
        ChangeStreetLights(LightState.on);
        ChangeStreetLights(LightState.flicker);
        timeOfDay = 0.0f;
        thunder = true;
        rainIntensity = 1.0f;
        allowAutoTime = false;
        skyboxOverride = true;
        skyboxOverrideVal = 1.84f;

        ambientDayColor = fear_AmbientDayColor;
        ambientNightColor = fear_AmbientNightColor;
    }
    void Anger()
    {
        ChangeStreetLights(LightState.redsky);
        timeOfDay = 6f;
        thunder = true;
        rainIntensity = 0.0f;
        allowAutoTime = false;
        skyboxOverride = true;
        skyboxOverrideVal = 5.0f;

        ambientDayColor = anger_AmbientDayColor;
        ambientNightColor = anger_AmbientNightColor;
    }


    //Day-Night Cycle Effects and Weather
    void TimeOfDay()
    { 
        //SET VALUES FOR SUNRISE AND SUNSET END
        sunriseEnd = sunrise + sunriseLength;
        sunsetEnd = sunset + sunsetLength;

        //UPDATE TIME OF DAY IF TIME IS SET TO AUTO
        if (autoTime)
            timeOfDay += 1.0f * timeScale * Time.deltaTime;

        //CHECK IF SUNRISE AND LERP BETWEEN NIGHT AND DAY
        if (timeOfDay > sunrise && timeOfDay < (sunrise + sunriseLength))
        {
            float lerpAmount = 1 - ((sunriseEnd - timeOfDay) / sunriseLength);
            RandomRain();
            RandomThunder();
            skyboxValue = 2.0f - (((timeOfDay - sunrise) / sunriseLength) * 2);
            RenderSettings.ambientSkyColor = Color.Lerp(ambientNightColor, ambientDayColor, lerpAmount);
            DynamicGI.UpdateEnvironment();
            isSunrise = true;
            isDaytime = false;
            isNighttime = false;
        }
        else
            isSunrise = false;

        //CHECK IF SUNSET AND LERP BETWEEN DAY AND NIGHT
        if (timeOfDay > sunset && timeOfDay < (sunset + sunsetLength))
        {
            float lerpAmount = 1 - ((sunsetEnd - timeOfDay) / sunsetLength);
            RandomRain();
            RandomThunder();
            skyboxValue = ((timeOfDay - sunset) / sunsetLength) * 2;
            RenderSettings.ambientSkyColor = Color.Lerp(ambientDayColor, ambientNightColor, lerpAmount);
            DynamicGI.UpdateEnvironment();
            isSunset = true;
            isDaytime = false;
            isNighttime = false;
        }
        else
            isSunset = false;

        //CHECK IF DAYTIME AND APPLY DAYTIME VALUES
        if (timeOfDay >= sunriseEnd && timeOfDay <= sunset)
        {
            skyboxValue = 0.0f;
            RenderSettings.ambientSkyColor = ambientDayColor;
            DynamicGI.UpdateEnvironment();
            isDaytime = true;
            isNighttime = false;
        }
        //CHECK IF NIGHTTIME AND APPLY NIGHTTIME VALUES
        else if((timeOfDay <= sunrise && timeOfDay >= 0.0f) || (timeOfDay >= sunsetEnd && timeOfDay <= 24.0f))
        {
            skyboxValue = 2.0f;
            RenderSettings.ambientSkyColor = ambientNightColor;
            DynamicGI.UpdateEnvironment();
            isDaytime = false;
            isNighttime = true;
        }

        //CANCEL RANDOM RAIN TOGGLE AFTER SUNSET AND SUNRISE
        if (!isSunset && !isSunrise)
        {
            rainToggle = false;
            thunderToggle = false;
        }

        //RESET TIME AFTER 24 HOURS
        if (timeOfDay > 24.0f)
            timeOfDay = 0.0f;
    }

    void ChangeSkybox()
    {
        if (skyboxOverride)
            skyboxValue = skyboxOverrideVal;

        blend1 = skyboxValue;
        skybox.SetFloat("_Blend1", blend1);

        if (skyboxValue >= 1.0f && skyboxValue <= 1.95f)
        {
            blend2 = skyboxValue - 1;
            skybox.SetFloat("_Blend2", blend2);
        }
        else if (skyboxValue < 1.0f)
        {
            blend2 = 0;
            skybox.SetFloat("_Blend2", blend2);
        }
    }

    void ChangeStreetLights(LightState _lightState)
    {
        foreach (GameObject light in streetLights)
        {
            light.GetComponent<StreetLightScript>().overrideState = _lightState;
        }
    }

    void ChangeRain()
    {
        if (thunder)
        {
            Thunder();
            rain.GetComponent<RainScript>().RainIntensity = rainIntensity;
        }
        else
        {
            rain.GetComponent<RainScript>().RainIntensity = rainIntensity;
            thunderReset = false;
        }
    }

    void RandomRain()
    {
        if (randomRain && !rainToggle)
        {
            float rand = Random.Range(0.0f, 100.0f);
            if (rand < 30.0f)
            {
                rainIntensity = Random.Range(0.0f, 1.0f);
            }
            else
            {
                rainIntensity = 0.0f;
            }
            rainToggle = true;
        }
    }
    void RandomThunder()
    {
        if (randomThunder && !thunderToggle)
        {
            float rand = Random.Range(0.0f, 100.0f);
            if (rand < 30.0f)
            {
                thunder = true;
            }
            else
            {
                thunder = false;
            }
            thunderToggle = true;
        }
    }

    void Thunder()
    {
        if (thunder)
        {
            thunderTimer += Time.deltaTime;

            if (!thunderReset) {
                randomThunderVal = Random.Range(5.0f, 10.0f);
                thunderReset = true;
            }

            if (thunderTimer > randomThunderVal)
            {
                if (!thunderFlash)
                {
                    skybox.SetFloat("_Blend1", 0.0f);
                    skybox.SetFloat("_Blend2", 0.0f);
                    GetComponent<AudioSource>().pitch = Random.Range(0.5f, 1.5f);
                    GetComponent<AudioSource>().volume = Random.Range(0.8f, 1.0f);
                    GetComponent<AudioSource>().PlayOneShot(thunderCrack);
                    directionalLight.GetComponent<DirectionalLightScript>().FlashThunder();
                    RenderSettings.ambientSkyColor = ambientDayColor;
                    foreach (GameObject light in houseSpotlights)
                    {
                        light.GetComponent<SpotLights>().FlashThunder();
                    }
                    thunderFlash = true;
                }
                if(thunderFlash && thunderTimer > randomThunderVal + 1.0f)
                {
                    if(Random.Range(0.0f, 100.0f) < 5.0f)
                    {
                        thunderFlash = false;
                    }
                }

                if (thunderTimer > randomThunderVal + 1.1f && thunderFlash)
                {
                    GetComponent<AudioSource>().pitch = 1.0f;
                    GetComponent<AudioSource>().volume = 1.0f;
                    RenderSettings.ambientSkyColor = ambientNightColor;
                    skybox.SetFloat("_Blend1", blend1);
                    skybox.SetFloat("_Blend2", blend2);
                    thunderReset = false;
                    thunderFlash = false;
                    thunderTimer = 0.0f;
                }
            }
        }
    }


    //Keyboard Listener
    void Keyboard()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            autoTime = !autoTime;
        }
    }
}
