using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : MonoBehaviour {

    public DoorOpen door;
    public GameObject effects;
    GameObject player;

	// Use this for initialization
	void Start () {
        player = GameObject.Find("ThirdPersonController");
    }
	
	// Update is called once per frame
	void Update () {
		if (((Vector3.Distance(player.transform.position, transform.position) < 5) && (door.BlockFound)) || door.BlockFound)
        {

        }
	}
}
