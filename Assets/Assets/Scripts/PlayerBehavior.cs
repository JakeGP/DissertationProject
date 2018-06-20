using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;
using UnityEngine.PostProcessing;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PostProcessingBehaviour))]
public class PlayerBehavior : MonoBehaviour {

    [HideInInspector] public bool MouseToggle = false;
    [HideInInspector] public bool Wakeup = true;
    [HideInInspector] public bool Standup = false;

    Animator CameraAnim;
    GameObject AnimationCamera;
    GameObject MainCamera;
    GameObject LoadingInstructions;
    Camera UICamera;
    Texture arrow;
    GameObject PlayerUI;
    Rect UIRect;

    GameManager manager;

    public string Objective;
    public float ObjectiveID;
    private float lerpTime;
    private float subtitleTimeout = -1.0f;
    private float objectiveAnimation = -1.0f;
    [HideInInspector] public float VignetteIntensity = 0.0f;

    public int PlayerState = 1;

    PostProcessingProfile m_Profile;
    public AudioMixer mixer;

    void OnEnable()
    {
        var behaviour = transform.Find("CameraMovement").Find("MainCamera").GetComponent<PostProcessingBehaviour>();

        if (behaviour.profile == null)
        {
            enabled = false;
            return;
        }

        m_Profile = Instantiate(behaviour.profile);
        behaviour.profile = m_Profile;
    }

    private void Awake()
    {
        transform.Find("PlayerUI").Find("FillScreen").GetComponent<Image>().color = new Color(0, 0, 0, 1);
    }

    // Use this for initialization
    void Start() {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        MainCamera = GameObject.Find("MainCamera");
        CameraAnim = MainCamera.GetComponent<Animator>();
        UICamera = GameObject.Find("UICamera").GetComponent<Camera>();
        PlayerUI = transform.Find("PlayerUI").gameObject;
        UIRect = PlayerUI.GetComponent<RectTransform>().rect;
        LoadingInstructions = PlayerUI.transform.Find("LoadingInstructions").gameObject;
        LoadingInstructions.SetActive(false);
        Wakeup = true;
    }
	
	// Update is called once per frame
	void Update () {
        Animate();
        GetObjective();
        Arrow();
        ShowTemporarySubtitle();
        AnimateObjective();
    }

    void Animate()
    {
        mixer.SetFloat("InsideSoundLowPass", Mathf.Lerp(0000.0f, 22000.0f, Time.timeSinceLevelLoad / 20.0f));
        ExitGameCheck();

        if (PlayerState == 1)
        {
            if (Wakeup)
            {
                IntroFadeIn();
                WakeUp();
            }
        }
        if(PlayerState == 2)
        {
            KeyboardCheck();
        }
        if (PlayerState == 3)
        {
            if(Standup)
            {
                StandUp();
                manager.currentObjective = "Turn off alarm";
            }
        }

        if(PlayerState == 4)
        {
            lerpTime = 0.0f;
            EnableAllControl();
            if (HasMouseMoved() && !Wakeup)
            {
                MouseToggle = true;
                DisableAnimation();
            }
            else
            {
                if (MouseToggle)
                {
                    MouseToggle = false;
                    EnableAnimation();
                }
            }
        }
        if(PlayerState == 999)
        {
            DisableAllControl();
            OutroFadeIn();
        }
    }

    void WakeUp()
    {
        CameraAnim.SetBool("wakeup", true);
        gameObject.GetComponent<RigidbodyFirstPersonController>().enabled = false;
        UICamera.enabled = false;
        PlayerUI.transform.Find("Objective").gameObject.SetActive(false);
    }
    void StandUp()
    {
        if(transform.localRotation.eulerAngles != Vector3.zero)
        {
            
        }
        gameObject.GetComponent<RigidbodyFirstPersonController>().enabled = false;
        CameraAnim.SetBool("standup", true);
        CameraAnim.SetBool("wakeup", false);
    }
    void EnableAllControl()
    {
        gameObject.GetComponent<RigidbodyFirstPersonController>().enabled = true;
        UICamera.enabled = true;
        PlayerUI.transform.Find("Objective").gameObject.SetActive(true);
    }
    void DisableAllControl()
    {
        gameObject.GetComponent<RigidbodyFirstPersonController>().enabled = false;
        UICamera.enabled = false;
        PlayerUI.transform.Find("Objective").gameObject.SetActive(false);
    }

    void EnableAnimation()
    {
        CameraAnim.StopPlayback();
    }
    void DisableAnimation()
    {
        CameraAnim.StartPlayback();
    }

    void GetObjective()
    {
        if (Objective != manager.currentObjective)
        {
            Objective = manager.currentObjective;
            PlayerUI.transform.Find("Objective").GetChild(0).GetComponent<Text>().text = Objective;
            objectiveAnimation = 0.0f;
        }
    }

    bool HasMouseMoved()
    {  
        return (Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0);
    }

    private void Arrow()
    {
        bool onScreen = false;
        GameObject arrow = PlayerUI.transform.Find("Objective").Find("Arrow").gameObject;

        if (manager.currentWaypointID >= 0 && manager.currentWaypointID < manager.waypoints.Length)
        {
            Vector3 viewPos = Camera.main.WorldToScreenPoint(manager.waypoints[manager.currentWaypointID].GetComponentInParent<Transform>().position);


            if (viewPos.x < 10.0f)
                viewPos.x = 10.0f;
            if (viewPos.y < 10.0f)
                viewPos.y = 10.0f;
            if (viewPos.x > UIRect.width - 10.0f)
                viewPos.x = UIRect.width - 10.0f;
            if (viewPos.y > UIRect.height - 10.0f)
                viewPos.y = UIRect.height - 10.0f;


            if (viewPos.x > 10.0f && viewPos.x < UIRect.width - 10.0f && viewPos.y > 10.0f && viewPos.y < UIRect.height - 10.0f)
                onScreen = true;

            float angle;

            if (viewPos.x > 240)
                angle = 90;
            else
                angle = 270;

            if (onScreen)
            {
                arrow.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            }
            else
            {
                arrow.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                arrow.GetComponent<RectTransform>().SetPositionAndRotation(viewPos, Quaternion.Euler(0, 0, angle));
            }
        }
        else
        {
            arrow.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }
    }

    public void SetSubtitle(string subtitle)
    {
        if(subtitle.Contains("instruction"))
        {
            PlayerUI.transform.Find("Subtitle").GetComponent<Text>().fontSize = 18;
            PlayerUI.transform.Find("Subtitle").GetComponent<Text>().fontStyle = FontStyle.BoldAndItalic;
            PlayerUI.transform.Find("Subtitle").GetComponent<Text>().text = subtitle.Replace("instruction-", "");
        }
        else
        {
            PlayerUI.transform.Find("Subtitle").GetComponent<Text>().fontSize = 22;
            PlayerUI.transform.Find("Subtitle").GetComponent<Text>().fontStyle = FontStyle.Normal;
            PlayerUI.transform.Find("Subtitle").GetComponent<Text>().text = subtitle;
        }
    }

    public void SetWaypointSubtitle(string subtitle)
    {
        if (subtitle.Contains("instruction"))
        {
            PlayerUI.transform.Find("Subtitle").GetComponent<Text>().fontSize = 18;
            PlayerUI.transform.Find("Subtitle").GetComponent<Text>().fontStyle = FontStyle.BoldAndItalic;
            PlayerUI.transform.Find("Subtitle").GetComponent<Text>().text = subtitle.Replace("instruction-", "");
        }
        else
        {
            PlayerUI.transform.Find("Subtitle").GetComponent<Text>().fontSize = 22;
            PlayerUI.transform.Find("Subtitle").GetComponent<Text>().fontStyle = FontStyle.Normal;
            PlayerUI.transform.Find("Subtitle").GetComponent<Text>().text = subtitle;
        }
        subtitleTimeout = 0.0f;
    }

    private void ShowTemporarySubtitle()
    {
        if(subtitleTimeout >= 0.0f)
        {
            subtitleTimeout += Time.deltaTime;
        }
        if(subtitleTimeout >= 8.0f)
        {
            PlayerUI.transform.Find("Subtitle").GetComponent<Text>().text = "";
            subtitleTimeout = -1.0f;
        }
    }

    public void Vignette(float intensity)
    {
        var vignette = m_Profile.vignette.settings;
        //vignette.intensity = Mathf.Abs(Mathf.Sin(Time.realtimeSinceStartup) * 0.99f) + 0.01f;
        vignette.intensity = intensity;
        m_Profile.vignette.settings = vignette;
    }
    public void IntroFadeIn()
    {
        lerpTime += Time.deltaTime;
        PlayerUI.transform.Find("FillScreen").GetComponent<Image>().color = new Color(0, 0, 0, Mathf.Lerp(1, 0, lerpTime / 5.0f));
    }
    public void OutroFadeIn()

    {
        lerpTime += Time.deltaTime;
        PlayerUI.transform.Find("FillScreen").GetComponent<Image>().color = new Color(0, 0, 0, Mathf.Lerp(0, 1, lerpTime / 2.0f));
        mixer.SetFloat("MasterVolume", Mathf.Lerp(0.0f, -40.0f, lerpTime / 2.0f));
        if (lerpTime > 4.0f)
        {
            LoadingInstructions.SetActive(true);
            StartCoroutine(manager.ASyncLoadGame());
            PlayerState = 0;
        }
    }

    public void SetPlayerState(int state)
    {
        PlayerState = state;
    }

    public void KeyboardCheck()
    {
        if(Input.GetKeyDown(KeyCode.E) || Input.GetButton("JoystickA"))
        {
            if(PlayerState == 2)
            {
                PlayerState = 3;
                Standup = true;
            }
        }
    }
    public void ExitGameCheck()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PlayerState = 999;
        }
    }


    void AnimateObjective()
    {
        if(objectiveAnimation >= 0.0f)
        {
            objectiveAnimation += Time.deltaTime;
            if (objectiveAnimation <= 0.5f)
            {
                float val = Mathf.Lerp(1, 1.2f, objectiveAnimation / 0.5f);
                PlayerUI.transform.Find("Objective").GetComponent<RectTransform>().localScale = new Vector3(val, val, val);
            }
            else
            {
                float val = Mathf.Lerp(1, 1.2f, 2 - (objectiveAnimation / 0.5f));
                PlayerUI.transform.Find("Objective").GetComponent<RectTransform>().localScale = new Vector3(val, val, val);
            }
        }
        if(objectiveAnimation > 1.0f) {
            objectiveAnimation = -1.0f;
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "Interior")
            manager.isInside = true;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.name == "Interior")
            manager.isInside = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Interior")
            manager.isInside = false;
    }

}
