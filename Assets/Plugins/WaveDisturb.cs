using UnityEngine;
using System.Collections;


public class WaveDisturb : MonoBehaviour {
	
	static float collibrate = (1024f-768f)/2f;
	
	[HideInInspector]
	public Vector2 pos;
	[HideInInspector]
	public float radius;
	[HideInInspector]
	public bool inWave = false;
	
	private float scale = 1;
	
	// Use this for initialization
	void Start () {
		
		pos.x = transform.position.x;
		pos.y = transform.position.y+collibrate;
		
		PackedSprite ps = gameObject.GetComponent<PackedSprite>();
		
		radius = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
		if (inWave)
		{
			scale = scale + 0.01f*Mathf.Sin(30*Time.timeSinceLevelLoad);
			
			inWave = false;
			
			Vector3 sc = transform.localScale;
			sc.x = scale;
			sc.y = scale;
			transform.localScale = sc;
			
		} else {
			if (scale !=1)
			{
				scale = Mathf.Lerp(scale,1,5*Time.deltaTime);
				if (Mathf.Abs(scale-1) < 0.01)
					scale = 1;
				Vector3 sc = transform.localScale;
				sc.x = scale;
				sc.y = scale;
				transform.localScale = sc;
			}
		}
	}
}
