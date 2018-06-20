using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaypointScript : MonoBehaviour {

    public int waypointID;
    public int nextWaypointID;

    public Vector3 startingScale;
    public Vector3 maxScale;

    GameObject player;
    GameManager manager;
    GameObject objectiveText;

    float distance;
    public float distanceToTrigger = 4;
    [HideInInspector]public bool objectiveMet = false;
    public string objective;

    public bool isActive = false;
    bool isFading = false;

    public bool triggerSubtitle = false;
    public string newSubtitle;

    float lerpTime = 0.0f;

	// Use this for initialization
	void Start () {
        player = GameObject.Find("Player");
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        startingScale = transform.localScale;
        objectiveText = transform.Find("Objective").gameObject;
        objectiveText.GetComponent<Text>().text = objective;
        EnableDisable();
	}

    // Update is called once per frame
    void Update() {
        CheckPlayerDistance();
        CheckIfObstructed();
    }

    private void CheckPlayerDistance()
    {
        distance = Vector3.Distance(transform.position, player.transform.position);
        Vector3 v3 = player.transform.position - transform.position;
        v3.y = 0.0f;
        transform.rotation = Quaternion.LookRotation(-v3);

        transform.GetChild(0).gameObject.GetComponent<Text>().text = Mathf.RoundToInt(distance) + "m";

        if (distance > 5 && distance < 100)
        {
            transform.localScale = Vector3.Lerp(startingScale, maxScale, distance / 100);
        }

        if (isActive)
        {
            if (distance < 15)
            {
                objectiveText.GetComponent<Text>().enabled = true;
            }
            else
            {
                objectiveText.GetComponent<Text>().enabled = false;
            }

            if (distance < distanceToTrigger)
            {
                if (!objectiveMet)
                {
                    lerpTime = 0.0f;
                    objectiveMet = true;
                    if (triggerSubtitle)
                    {
                        player.GetComponent<PlayerBehavior>().SetWaypointSubtitle(newSubtitle);
                    }
                }
            }
        }

        isFading = false;
        if (!isActive && objectiveMet)
        {
            HideWaypoint();
        }
        else if (isActive)
        {
            ShowWaypoint();
        }
        
    }
                                  
    public void EnableDisable()
    {
        if(isActive)
        {
            GetComponent<RawImage>().color = new Color(1, 1, 1, 1);
            transform.GetChild(0).GetComponent<Text>().color = new Color(1, 1, 1, 1);
            transform.GetChild(1).GetComponent<Text>().color = new Color(1, 1, 1, 1);
        }
        else
        {
            GetComponent<RawImage>().color = new Color(1, 1, 1, 0);
            transform.GetChild(0).GetComponent<Text>().color = new Color(1, 1, 1, 0);
            transform.GetChild(1).GetComponent<Text>().color = new Color(1, 1, 1, 0);
        }
    } 
                                  
    public void HideWaypoint()
    {
        lerpTime += Time.deltaTime;
        GetComponent<RawImage>().color = new Color(1, 1, 1, Mathf.Lerp(1, 0, lerpTime / 1.0f));
        transform.GetChild(0).GetComponent<Text>().color = new Color(1, 1, 1, Mathf.Lerp(1, 0, lerpTime / 1.0f));
        transform.GetChild(1).GetComponent<Text>().color = new Color(1, 1, 1, Mathf.Lerp(1, 0, lerpTime / 1.0f));
        if(lerpTime < 1.0f)
        {
            isFading = true;
        }
    }

    public void ShowWaypoint()
    {
        lerpTime += Time.deltaTime;
        GetComponent<RawImage>().color = new Color(1, 1, 1, Mathf.Lerp(0, 1, lerpTime / 1.0f));
        transform.GetChild(0).GetComponent<Text>().color = new Color(1, 1, 1, Mathf.Lerp(0, 1, lerpTime / 1.0f));
        transform.GetChild(1).GetComponent<Text>().color = new Color(1, 1, 1, Mathf.Lerp(0, 1, lerpTime / 1.0f));
        if (lerpTime < 1.0f)
        {
            isFading = true;
        }
    }

    public void ToggleState()
    {
        if(isActive)
        {
            isActive = false;
        }
        else
        {
            isActive = true;
        }
    }

    public void CheckIfObstructed()
    {
        if (!isFading && isActive)
        {
            RaycastHit hit = new RaycastHit();
            Vector3 rayDirection = player.transform.position - transform.position;
            if (Physics.Raycast(transform.position, rayDirection, out hit))
            {
                if (hit.transform == player.transform)
                {
                    GetComponent<RawImage>().color = new Color(1, 1, 1, 1);
                    transform.GetChild(0).GetComponent<Text>().color = new Color(1, 1, 1, 1);
                    transform.GetChild(1).GetComponent<Text>().color = new Color(1, 1, 1, 1);
                }
                else
                {
                    GetComponent<RawImage>().color = new Color(1, 1, 1, 0.3f);
                    transform.GetChild(0).GetComponent<Text>().color = new Color(1, 1, 1, 0.3f);
                    transform.GetChild(1).GetComponent<Text>().color = new Color(1, 1, 1, 0.3f);
                }
            }
        }
    }
}
