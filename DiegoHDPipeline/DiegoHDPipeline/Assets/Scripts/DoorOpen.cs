using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorOpen : MonoBehaviour {

    public Image FillBar1;
    public Image FillBar2;
    public Image FillRing;
    public Image DarkRing;
    public ParticleSystem LightParts;

    public Transform Wing1, Wing2;
    public Transform Disc;
    public Transform Door;

    public Transform LerpWing1, LerpWing2;
    public Transform LerpDoor;
    public Transform LerpBlock1, LerpBlock2;

    public float WingSpeed = 0.1f;
    public float FillSpeed = 0.1f;
    public float DoorSpeed = 0.1f;
    public float DiscSpeed = 0.1f;
    public float BlockSpeed = 0.1f;
    public float WingsDelay = 0.5f;
    public float doorDelay = 0.5f;

    public bool barActivated = false;
    public LoadingBar bar;

    Transform Block;

    [HideInInspector]
    public bool BlockFound = false;
    bool Activated = false;
    bool WingsOpening = false;
    bool WingsOpen = false;
    bool DoorOpening = false;
    bool DoorOpened = false;

    // Use this for initialization
    void Start () {
        FillSpeed = FillSpeed / 100;
        WingSpeed = WingSpeed / 100;
        DoorSpeed = DoorSpeed / 100;
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.G))
        {
            Activated = true;
        }

        if (BlockFound && !Activated && !DoorOpened && !barActivated)
        {
            Block.gameObject.GetComponent<Rigidbody>().useGravity = false;
            Block.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            Block.gameObject.GetComponent<Animator>().enabled = false;
            Block.position = Vector3.Lerp(Block.position, LerpBlock1.position, BlockSpeed);
            //Block.eulerAngles = Vector3.Lerp(Block.eulerAngles, new Vector3(0,0,0), 0.01f);
            Block.rotation = Quaternion.Slerp(Block.rotation, Quaternion.identity, 0.05f);
            if (Vector3.Distance(Block.position, LerpBlock1.position) < 0.05f && Quaternion.Angle(Block.rotation,Quaternion.identity) < 1.5f)
            {
                Block.parent = Disc;
                Activated = true;
            }
        }

        if (barActivated)
        {
            if (bar.currentFillNum > 0.97f)
            {
                Activated = true;
            }
        }

		if (Activated)
        {
            if (FillRing.fillAmount < 1)
            {
                FillRing.fillAmount += FillSpeed;
            }
            else
            {
                DarkRing.gameObject.SetActive(false);
                if ((FillBar1.fillAmount < 1 || FillBar2.fillAmount < 1) && !WingsOpening)
                {
                    FillBar1.fillAmount += FillSpeed;
                    FillBar2.fillAmount += FillSpeed;
                }
                else if (WingsOpening == false)
                {
                    Invoke("OpenWings", WingsDelay);
                }
            }

            if (WingsOpening && !WingsOpen)
            {
                Disc.Rotate(0,DiscSpeed,0);
                Wing1.position = Vector3.Lerp(Wing1.position, LerpWing1.position, WingSpeed);
                Wing2.position = Vector3.Lerp(Wing2.position, LerpWing2.position, WingSpeed);
                if (!barActivated)
                {
                    if (Vector3.Distance(Block.position, LerpBlock2.position) > 0.03f)
                    {
                        Block.position = Vector3.Lerp(Block.position, LerpBlock2.position, (BlockSpeed / 10));
                    }
                }
                if (Vector3.Distance(Wing1.position, LerpWing1.position) < 3f && Vector3.Distance(Wing2.position, LerpWing2.position) < 3f
                    && (barActivated || (Block != null && Vector3.Distance(Block.position, LerpBlock2.position)  < 0.03f)) && (Disc.localEulerAngles.y % 45 < 0.5f))
                {
                    //print(Disc.eulerAngles.y);
                    WingsOpen = true;
                    Invoke("OpenDoor", doorDelay);
                }
            }

            if (DoorOpening && !DoorOpened)
            {
                Door.position = Vector3.Lerp(Door.position, LerpDoor.position, DoorSpeed);
                if (Vector3.Distance(Door.position, LerpDoor.position) < 0.1f)
                {
                    DoorOpened = true;
                    Activated = false;
                }
            }
        }
	}

    void OpenWings()
    {
        if (!WingsOpening)
        {
            WingsOpening = true;
            LightParts.gameObject.SetActive(true);
            LightParts.Play();
            Invoke("HideBars",0.15f);
            Invoke("DestroyParts", 4);
        }
    }

    void OpenDoor()
    {
        DoorOpening = true;
    }

    void HideBars()
    {
        FillBar1.fillAmount = 0;
        FillBar2.fillAmount = 0;
    }

    void DestroyParts()
    {
        LightParts.Stop();
        LightParts.gameObject.SetActive(false);
    }

    void OnTriggerStay(Collider hit)
    {
        if (!BlockFound && !barActivated)
        {
            if (hit.gameObject.tag == "EnergySource")
            {
                Block = hit.transform;
                BlockFound = true;
                Block.gameObject.GetComponent<Rigidbody>().useGravity = false;
                Block.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                Block.gameObject.GetComponent<BoxCollider>().enabled = false;
                Block.gameObject.GetComponent<LiftableObject>().Interactions.dropObject();
                Block.gameObject.GetComponent<LiftableObject>().enabled = false;
                //Block.tag = "Default";
            }
        }
    }
}
