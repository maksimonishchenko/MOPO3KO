using UnityEngine;
using System.Collections;

public class RendererSwitcher : MonoBehaviour 
{

	
	IEnumerator Start() 
	{
        
        yield return new WaitForSeconds(0.25f);
        renderer.enabled = true;
    }
	
	
}
