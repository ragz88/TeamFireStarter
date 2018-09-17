using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarActivator : MonoBehaviour {

	//public Image[] bars;
	public LoadingBar[] bars;

    public bool testActivator = false;
    public float startIntensity = 1;
    public float endIntensity = 60;
    public float intensityChangeSpeed = 0.25f;

    bool isActive = false;
    float padIntensity = 1;

    public Renderer ActivatorPadRend;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (testActivator)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                for (int i = 0; i < bars.Length; i++)
                {
                    bars[i].Fill();
                }
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                for (int i = 0; i < bars.Length; i++)
                {
                    bars[i].Empty();
                }
            }
        }

        if (isActive)
        {
            if (padIntensity < endIntensity)
            {
                padIntensity += intensityChangeSpeed;
                ActivatorPadRend.material.SetFloat("Vector1_56A74A91", padIntensity);      //this is the fresnel strength
            }
        }
        else
        {
            if (padIntensity > startIntensity)
            {
                padIntensity -= intensityChangeSpeed;
                ActivatorPadRend.material.SetFloat("Vector1_56A74A91", padIntensity);      //this is the fresnel strength
            }
        }


	}

    void OnTriggerEnter(Collider hit)
    {
        if (hit.tag == "EnergySource")
        {
            for (int i = 0; i < bars.Length; i++)
            {
                bars[i].Fill();
            }
            isActive = true;
        }
        
    }

    private void OnTriggerExit(Collider hit)
    {
        if (hit.tag == "EnergySource")
        {
            for (int i = 0; i < bars.Length; i++)
            {
                bars[i].Empty();
            }
            isActive = false;
        }
    }
}
