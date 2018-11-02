using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour { 

    public int pushBlockWallLayer;
    public int[] ignoredByPushBlockWalls;
    public ThirdPersonOrbitCamBasic cam1;
    public AdvancedCamera cam2;

    // Use this for initialization
    void Start () {

        for (int i = 0; i < ignoredByPushBlockWalls.Length; i++)
        {
            Physics.IgnoreLayerCollision(pushBlockWallLayer, ignoredByPushBlockWalls[i], true);
        }
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        /*if (Input.GetKeyDown(KeyCode.C))
        {
            if (cam1.enabled == true)
            {
                cam2.enabled = true;
                cam1.enabled = false;
            }
            else
            {
                cam2.enabled = false;
                cam1.enabled = true;
            }
            
        }*/

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneManager.LoadScene(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SceneManager.LoadScene(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SceneManager.LoadScene(2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SceneManager.LoadScene(3);
        }

        /*if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SceneManager.LoadScene(4);
        }*/

    }
}
