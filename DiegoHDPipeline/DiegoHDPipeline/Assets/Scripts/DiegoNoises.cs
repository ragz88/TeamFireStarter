using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class DiegoNoises : MonoBehaviour {

    AudioSource diego;
    public AudioClip walkSound;
    public AudioClip jumpSound;
    // Use this for initialization
    void Start ()
    {
        diego.playOnAwake = false;
        diego = this.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlayWalk()
    {
        diego.Stop();
        diego.clip = walkSound;
        diego.Play();
    }

    public void PlayJump()
    {
        diego.Stop();
        diego.clip = jumpSound;
        diego.Play();
    }
}
