using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingBar : MonoBehaviour {

	public Image bar;
	public float currentFillNum = 0;
	public bool filling = false;
	public bool emptying = false;
	public float fillSpeed= 10f;
    public float emptySpeed = 20f;
	public LoadingBar[] linkedBars;

	// Use this for initialization
	void Start () {
		if (bar.fillMethod == Image.FillMethod.Horizontal)
		{
			fillSpeed = (fillSpeed * 0.01f)/bar.rectTransform.rect.width;
            emptySpeed = (emptySpeed * 0.01f) / bar.rectTransform.rect.width;
        }
		else if (bar.fillMethod == Image.FillMethod.Vertical)
		{
			fillSpeed = (fillSpeed * 0.01f) / bar.rectTransform.rect.height;
            emptySpeed = (emptySpeed * 0.01f) / bar.rectTransform.rect.height;
        }

		currentFillNum = bar.fillAmount;
	}
	
	// Update is called once per frame
	void Update () {

		if (filling)
		{
			if (currentFillNum < 1)
			{
				currentFillNum = currentFillNum + fillSpeed;
				bar.fillAmount = currentFillNum;
			}
			else
			{
				for (int i = 0; i < linkedBars.Length; i++)
				{
					linkedBars [i].Fill();
				}
                filling = false;
			}
		}
		else
		{
            if (emptying)
			{
                bool emptyLinkedBarFound = false;
                if (linkedBars.Length > 0)
                {
                    for (int i = 0; i < linkedBars.Length; i++)
                    {
                        if (linkedBars[i].currentFillNum <= 0)
                        {
                            emptyLinkedBarFound = true;
                        }
                    }
                }
                else
                {
                    emptyLinkedBarFound = true;
                }
                if (currentFillNum > 0 && emptyLinkedBarFound) 
				{
					currentFillNum = currentFillNum - emptySpeed;
					bar.fillAmount = currentFillNum;
				}
				else
				{
					for (int i = 0; i < linkedBars.Length; i++)
					{
						linkedBars [i].Empty();
					}
				}
			}
		}
	}

	public void Empty()
	{
		//if (filling == true && emptying == false)
		//{
		//	bar.fillOrigin = (bar.fillOrigin + 1) % 2;
		//}
		filling = false;
		emptying = true;
	}

	public void Fill()
	{
		//if (filling == false && emptying == true)
		//{
		//	bar.fillOrigin = (bar.fillOrigin + 1) % 2;
		//}
		filling = true;
		emptying = false;
	}

}
