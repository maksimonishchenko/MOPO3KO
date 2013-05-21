using UnityEngine;
using System.Collections;

public class WWWOpener : MonoBehaviour 
{
	
	public string button1LinkAdress;
	public string button2LinkAdress;
	
	
	void Start ()
	{
	
	}
	
	
	void Update ()
	{
	
	}
	
	public void Click1()
	{
		Application.OpenURL(button1LinkAdress);
	}
	
	public void Click2()
	{
		Application.OpenURL(button2LinkAdress);
	}
}
