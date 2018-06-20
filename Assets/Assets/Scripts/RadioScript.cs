using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class RadioScript : MonoBehaviour {

    Interactable state;
    AudioSource source;
    public AudioMixer mixer;

	// Use this for initialization
	void Start () {
        state = GetComponent<Interactable>();
        source = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		if(state.ObjectState)
        {
            mixer.SetFloat("RadioVol", -5.0f);
        }
        else
        {
            mixer.SetFloat("RadioVol", -80.0f);
        }
	}
}
