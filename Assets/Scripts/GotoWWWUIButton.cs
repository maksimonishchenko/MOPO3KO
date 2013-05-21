using UnityEngine;
using System.Collections;

public class GotoWWWUIButton : MonoBehaviour 
{
	public UIButton3D button;
	public string adress;
	
	
	// Use this for initialization
	void Start () 
	{
		if (null != button)
		{
			button.scriptWithMethodToInvoke = this;
			button.methodToInvoke = "Click";
		}
	}
	
	// Update is called once per frame
	void Click () 
	{
		if (null != adress)
		{
			Application.OpenURL(adress);	
		}
	}
}