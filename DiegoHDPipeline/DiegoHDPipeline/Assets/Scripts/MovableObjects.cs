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
    public float stoppingDistance = 0.05f;
    public float slowInSpeed = 3;
    public float slowOutSpeed = 4;
    public bool slowInSlowOut = true;

    public LoadingBar[] activatingBars;

    public Transform optionalReturnPos;

    int destinationNum = 0;
    Vector3 initPos;
    bool completedMovement = false;
    float initMoveSpeed;
    Vector3 lastPos;


	// Use this for initialization
	void Start () {
        lastPos = transform.position;
        initMoveSpeed = movementSpeed;
        initPos = transform.position;
        //movementSpeed = movementSpeed * 0.01f;
        movePoints = new Vector3[moveTransforms.Length];
        for (int i = 0; i < moveTransforms.Length; i++)
        {
            movePoints[i] = moveTransforms[i].position;
        }
	}
	
	// Update is called once per frame
	void Update () {

        if (optionalReturnPos != null)
        {
            initPos = optionalReturnPos.position;
        }

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
            //slow in Slow out code
            if (slowInSlowOut)
            {
                if (Vector3.Distance(lastPos, transform.position) < Vector3.Distance(movePoints[destinationNum], transform.position))
                {
                    if (movementSpeed < Vector3.Distance(lastPos, transform.position))
                    {
                        movementSpeed += (Time.deltaTime * slowOutSpeed);
                        if (movementSpeed > initMoveSpeed)
                        {
                            movementSpeed = initMoveSpeed;
                        }
                    }
                }
                else
                {
                    if (movementSpeed > Vector3.Distance(movePoints[destinationNum], transform.position))
                    {
                        movementSpeed -= (Time.deltaTime * slowInSpeed);
                        if (movementSpeed <= 0)
                        {
                            movementSpeed = Time.deltaTime * slowInSpeed;
                        }
                    }
                }
            }


            //transform.position = Vector3.Lerp(transform.position, movePoints[destinationNum],(movementSpeed * Time.deltaTime));
            transform.position = Vector3.MoveTowards(transform.position, movePoints[destinationNum], (movementSpeed * Time.deltaTime));
            if (Vector3.Distance(transform.position, movePoints[destinationNum]) < stoppingDistance)
            {
                lastPos = movePoints[destinationNum];
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
            else
            {

            }
        }
        else
        {
            if (returnOnInactive && !isActive)
            {
                if (Vector3.Distance(transform.position, initPos) > stoppingDistance)
                {
                    //transform.position = Vector3.Lerp(transform.position, initPos, (movementSpeed * Time.deltaTime));
                    transform.position = Vector3.MoveTowards(transform.position, initPos, (movementSpeed * Time.deltaTime));
                }
                else
                {
                    completedMovement = false; 
                }
            }
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerBody")
        {
            other.gameObject.transform.parent = transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "PlayerBody")
        {
            other.gameObject.transform.parent = null;
        }
    }
}
