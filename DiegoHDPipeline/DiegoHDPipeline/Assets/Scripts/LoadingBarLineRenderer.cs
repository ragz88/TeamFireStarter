using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingBarLineRenderer : MonoBehaviour {

	public LineRenderer bar;
	public int currentFillPoint = 1;
	public bool filling = false;
	public bool emptying = false;
	public float fillSpeed= 10f;
    public float emptySpeed = 20f;
	public LoadingBarLineRenderer[] linkedBars;
    public Transform[] barPoints;
    public Transform initialSource;
    public Vector3 lerpingPoint;
    public bool empty = true;
    public bool full = false;
    Vector3[] points;

    //public Transform debugTrans;

	// Use this for initialization
	void Start () {
        /*if (bar.fillMethod == Image.FillMethod.Horizontal)
		{
			fillSpeed = (fillSpeed )/bar.rectTransform.rect.width;
            emptySpeed = (emptySpeed ) / bar.rectTransform.rect.width;
        }
		else if (bar.fillMethod == Image.FillMethod.Vertical)
		{
			fillSpeed = (fillSpeed ) / bar.rectTransform.rect.height;
            emptySpeed = (emptySpeed ) / bar.rectTransform.rect.height;
        }*/

        //currentFillNum = bar.fillAmount;

        lerpingPoint = barPoints[0].position;
        points = new Vector3[barPoints.Length];
        points[0] = barPoints[0].position;
        for (int i = 0; i < barPoints.Length; i++)
        {
            bar.SetPosition(i,points[0]);
        }
        
	}
	
	// Update is called once per frame
	void Update () {
        //debugTrans.position = lerpingPoint;
        //print(currentFillPoint);
        if (Input.GetKeyDown(KeyCode.F))
        {
            Fill();
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            Empty();
        }

        if (filling)
		{
            empty = false;
            //float dist = Vector3.Distance(lerpingPoint, barPoints[currentFillPoint].position);

            if (Vector3.Distance(lerpingPoint, barPoints[currentFillPoint].position) > 0.001f && !full)
			{
                //lerpingPoint = Vector3.Lerp(barPoints[currentFillPoint - 1].position, barPoints[currentFillPoint].position, fillSpeed);
                lerpingPoint = Vector3.MoveTowards(lerpingPoint, barPoints[currentFillPoint].position, fillSpeed);

                for (int i = currentFillPoint; i <= points.Length - 1; i++)
                {
                    points[i] = lerpingPoint;
                }
			}
			else
			{
                if (currentFillPoint < barPoints.Length-1)
                {
                    currentFillPoint++;
                }
                else
                {
                    full = true;
                    for (int i = 0; i < linkedBars.Length; i++)
                    {
                        linkedBars[i].Fill();
                    }
                    filling = false;
                }	
			}
		}
		else
		{
            if (emptying)
			{
                full = false;
                bool emptyLinkedBarFound = false;
                if (linkedBars.Length > 0)
                {
                    for (int i = 0; i < linkedBars.Length; i++)
                    {
                        if (linkedBars[i].empty)
                        {
                            emptyLinkedBarFound = true;
                        }
                    }
                }
                else
                {
                    emptyLinkedBarFound = true;
                }

                if (!empty && emptyLinkedBarFound) 
				{
                    if (Vector3.Distance(lerpingPoint, barPoints[currentFillPoint].position) > 0.001f && !empty)
                    {
                        //lerpingPoint = Vector3.Lerp(barPoints[currentFillPoint - 1].position, barPoints[currentFillPoint].position, fillSpeed);
                        lerpingPoint = Vector3.MoveTowards(lerpingPoint, barPoints[currentFillPoint].position, emptySpeed);

                        for (int i = currentFillPoint; i <= points.Length - 1; i++)
                        {
                            points[i] = lerpingPoint;
                        }
                    }
                }
				else
				{
                    if (currentFillPoint > 0)
                    {
                        currentFillPoint--;
                    }
                    else
                    {
                        empty = true;
                        for (int i = 0; i < linkedBars.Length; i++)
                        {
                            linkedBars[i].Empty();
                        }
                    }
				}
			}
		}

        bar.SetPositions(points);
	}

	public void Empty()
	{
        //if (filling == true && emptying == false)
        //{
        //	bar.fillOrigin = (bar.fillOrigin + 1) % 2;
        //}
        if (!emptying)
        {
            currentFillPoint--;
        }
        filling = false;
		emptying = true;
        
	}

	public void Fill()
	{
		//if (filling == false && emptying == true)
		//{
		//	bar.fillOrigin = (bar.fillOrigin + 1) % 2;
		//}
		
        if (!filling)
        {
            currentFillPoint++;
        }
        filling = true;
        emptying = false;
    }

}
