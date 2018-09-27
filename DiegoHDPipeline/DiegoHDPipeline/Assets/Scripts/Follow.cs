using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour {

    public Transform ObjectToFollow;
    public bool Lerp = false;
    public float lerpSpeed = 0.1f;
    public bool followX = false;
    public bool followY = false;
    public bool followZ = false;

    float x, y, z;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        x = transform.position.x;
        y = transform.position.y;
        z = transform.position.z;

        if (followX)
        {
            x = ObjectToFollow.position.x;
        }

        if (followY)
        {
            y = ObjectToFollow.position.y;
        }

        if (followZ)
        {
            z = ObjectToFollow.position.z;
        }

        if (Lerp)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(x, y, z), lerpSpeed);
        }
        else
        {
            transform.position = new Vector3(x,y,z);
        }
    }
}
