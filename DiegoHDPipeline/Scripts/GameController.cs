using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour { 

    public int pushBlockWallLayer;
    public int[] ignoredByPushBlockWalls;

    // Use this for initialization
    void Start () {

        for (int i = 0; i < ignoredByPushBlockWalls.Length; i++)
        {
            Physics.IgnoreLayerCollision(pushBlockWallLayer, ignoredByPushBlockWalls[i], true);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
