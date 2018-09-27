using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableObject : MonoBehaviour {

    GameObject player;
    public bool beingPushed = false;
    public Transform[] pushTransforms;
    public Transform[] pushPoints;
    public float velocityPitchEffect = 0.05f;
    AudioSource pushSound;
    Rigidbody body;
    float initVolume;

	// Use this for initialization
	void Start () {
        player = GameObject.Find("ThirdPersonController");
        pushSound = gameObject.GetComponent<AudioSource>();
        body = gameObject.GetComponent<Rigidbody>();
        initVolume = pushSound.volume;
        pushSound.volume = 0.05f;
    }
	
	// Update is called once per frame
	void Update () {
        if (body.velocity.magnitude > 0.3f)
        {
            if (!pushSound.isPlaying)
            {
                pushSound.Play();
            }
            else
            {
                pushSound.pitch = 1 + (body.velocity.magnitude * velocityPitchEffect);
                if (pushSound.volume < initVolume)
                {
                    pushSound.volume += 0.15f;
                }
            }
        }
        else
        {
            if (pushSound.isPlaying)
            {
                if (pushSound.volume < 0.05f)
                {
                    pushSound.Pause();
                }
                else
                {
                    pushSound.volume -= 0.03f;
                }
            }
        }
	}

}
