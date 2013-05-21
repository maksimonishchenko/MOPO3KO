using UnityEngine;
using System.Collections;

public class PPOnSpaceReset : MonoBehaviour {

	
	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			PlayerPrefs.DeleteAll();
		}
	}
}
