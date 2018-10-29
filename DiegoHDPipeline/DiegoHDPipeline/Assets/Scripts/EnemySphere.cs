using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySphere : MonoBehaviour {

    public GameObject sphere;
    GameObject outerSphere;
    public NavMeshAgent agent;
    public float sphereAlignSpeed = 5;


    // Use this for initialization
    void Start () {
        outerSphere = gameObject;
        outerSphere.transform.parent = null;
    }

    // Update is called once per frame
    void Update () {
        outerSphere.transform.position = Vector3.Lerp(outerSphere.transform.position, sphere.transform.position, sphereAlignSpeed * Time.deltaTime);
        outerSphere.transform.RotateAround(outerSphere.transform.position, sphere.transform.right, agent.velocity.magnitude * 100 * Time.deltaTime);
    }
}
