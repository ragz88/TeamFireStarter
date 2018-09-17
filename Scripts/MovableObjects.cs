using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObjects : MonoBehaviour {

    public Transform[] moveTransforms;
    Vector3[] movePoints;
    public bool isLooping = false;
    public bool isActive = false;
    public bool returnOnInactive = false;
    public float movementSpeed = 0.1f;

    public LoadingBar[] activatingBars;

    int destinationNum = 0;
    Vector3 initPos;
    bool completedMovement = false;


	// Use this for initialization
	void Start () {
        initPos = transform.position;
        movementSpeed = movementSpeed * 0.01f;
        movePoints = new Vector3[moveTransforms.Length];
        for (int i = 0; i < moveTransforms.Length; i++)
        {
            movePoints[i] = moveTransforms[i].position;
        }
	}
	
	// Update is called once per frame
	void Update () {

        bool unfilledFound = false;
        for (int i = 0; i < activatingBars.Length; i++)
        {
            if (activatingBars[i].currentFillNum < 1)
            {
                unfilledFound = true;
            }
        }

        if (unfilledFound)
        {
            isActive = false;
        }
        else
        {
            isActive = true;
        }

        if (isActive && !completedMovement)
        {
            transform.position = Vector3.Lerp(transform.position, movePoints[destinationNum],movementSpeed);
            if (Vector3.Distance(transform.position, movePoints[destinationNum]) < 0.05f)
            {
                destinationNum++;
                if (destinationNum > movePoints.Length - 1)
                {
                    if (!isLooping)
                    {
                        isActive = false;
                        completedMovement = true;
                    }
                    destinationNum = 0;
                }
            }
        }
        else
        {
            if (returnOnInactive && !isActive)
            {
                if (Vector3.Distance(transform.position, initPos) > 0.05f)
                {
                    transform.position = Vector3.Lerp(transform.position, initPos, movementSpeed);
                }
                else
                {
                    completedMovement = false; 
                }
            }
        }
	}
}
