using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioFlicker : MonoBehaviour {

    MeshRenderer renderer;
    float timer = 0.0f;
    float randomVal = 0.0f;
    bool reset = false;

    Color min, max;
	// Use this for initialization
	void Start () {
        renderer = GetComponent<MeshRenderer>();

        ColorUtility.TryParseHtmlString("#809C1E", out min);
        ColorUtility.TryParseHtmlString("#CAF62F", out min);
    }
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;

        if(!reset)
        {
            randomVal = Random.Range(0.0f, 0.1f);
            reset = true;
        }

        if(timer > randomVal)
        {
            Color random = Color.Lerp(min, max, Random.Range(0.0f, 0.9f));
            renderer.materials[0].SetColor("_EmissionColor", random);
            timer = 0.0f;
            reset = false;
        }
	}
}
