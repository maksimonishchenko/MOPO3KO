using UnityEngine;
using System.Collections;

public class ProgresBarSampler : MonoBehaviour 
{

	public UIProgressBar bar;

	
	
	void Update () 
	{
		bar.Value+=Time.deltaTime;
	}
}
