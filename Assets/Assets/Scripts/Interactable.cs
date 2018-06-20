using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interactable : MonoBehaviour {

    public string ObjectTextOn;
    public string ObjectTextOff;

    public bool ObjectState = false;
    public bool OnlyTurnOff = false;

	// Use this for initialization
	void Start () {
        gameObject.tag = "Interactable";
        SetStateStart();
        AddDropShadow();
    }
	
	// Update is called once per frame
	void Update () {

    }

    public void AddDropShadow()
    {
        transform.Find("GUI").gameObject.AddComponent<Shadow>().effectDistance = new Vector2(-1, -1);

    }

    public void SetStateStart()
    {
        if (!ObjectState)
        {
            transform.Find("GUI").GetComponent<Text>().text = ObjectTextOn;
            ObjectState = false;
        }
        else
        {
            transform.Find("GUI").GetComponent<Text>().text = ObjectTextOff;
            ObjectState = true;
        }
    }

    public void ToggleState()
    {
        if (ObjectState)
        {
            transform.Find("GUI").GetComponent<Text>().text = ObjectTextOn;
            ObjectState = false;
        }
        else if(!ObjectState && !OnlyTurnOff)
        {
            transform.Find("GUI").GetComponent<Text>().text = ObjectTextOff;
            ObjectState = true;
        }
    }
}
