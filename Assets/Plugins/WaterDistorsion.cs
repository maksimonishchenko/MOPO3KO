

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum RIPPLE_TYPE {
    RIPPLE_TYPE_RUBBER,                                     // a soft rubber sheet
    RIPPLE_TYPE_GEL,                                        // high viscosity fluid
    RIPPLE_TYPE_WATER,                                      // low viscosity fluid
}

public class rippleData {
    public RIPPLE_TYPE             rippleType;                     // type of ripple ( se update: )
    public Vector2                 center;                         // ripple center ( but you just knew that, didn't you? )
    public Vector2                 centerCoordinate;               // ripple center in texture coordinates
    public float                   radius;                         // radius at which ripple has faded 100%
    public float                   strength;                       // ripple strength
    public float                   runtime;                        // current run time
    public float                   currentRadius;                  // current radius
    public float                   rippleCycle;                    // ripple cycle timing
    public float                   lifespan;                       // total life span
}

[RequireComponent (typeof (MeshRenderer))]
[RequireComponent (typeof (MeshFilter))]
[RequireComponent (typeof (Mesh))]
[ExecuteInEditMode]
public class WaterDistorsion : MonoBehaviour {
	
	protected float RIPPLE_BASE_GAIN = 0.1f;        // an internal constant

	protected int RIPPLE_DEFAULT_RADIUS = 800;         // radius in pixels
	protected float RIPPLE_DEFAULT_RIPPLE_CYCLE = 0.2f;        // timing on ripple ( 1/frequenzy )
	protected float RIPPLE_DEFAULT_LIFESPAN = 2.0f;        // entire ripple lifespan
	
	public int sectors = 32;
	public int mult = 32;
	
	private Mesh m_mesh;
	protected Vector3[] m_vertices;
	protected bool[] m_edgeVertice;
	protected int[] m_triangles;
	protected Vector2[] m_uvs;
	protected Vector2[] m_uv_copy;
	protected int m_bufferSize;
	
	[HideInInspector]
	public float collibrate = (1024f-768f)/2f;
	
	List<rippleData> m_rippleList = new List<rippleData>();
	
	private WaveDisturb[] disturb;
	
	
	void Awake() {
		initMesh();
	}
	
	// Use this for initialization
	void Start () {
		initDisturb();
	}
	
	public void initDisturb() {
	
		disturb = (WaveDisturb[])GameObject.FindObjectsOfType(typeof(WaveDisturb));
	
	}
	
	public void addRipple(Vector2 pos, RIPPLE_TYPE type, float strength) {
	    rippleData newRipple = new rippleData();
	
	
	    // initialize ripple
	    newRipple.rippleType = type;
	    newRipple.center = pos;
	    newRipple.centerCoordinate = new Vector2(pos.x/(mult*sectors),pos.y/(mult*sectors));
	    newRipple.radius = RIPPLE_DEFAULT_RADIUS;
	    newRipple.strength = strength;
	    newRipple.runtime = 0;
	    newRipple.currentRadius = 0;
	    newRipple.rippleCycle = RIPPLE_DEFAULT_RIPPLE_CYCLE;
	    newRipple.lifespan = RIPPLE_DEFAULT_LIFESPAN;
	
	    // add ripple to running list
		m_rippleList.Add(newRipple);	
	}
	
	public void addRipple(Vector2 pos, RIPPLE_TYPE type, float strength, float radius) {
	    rippleData newRipple = new rippleData();
	
	
	    // initialize ripple
	    newRipple.rippleType = type;
	    newRipple.center = pos;
	    newRipple.centerCoordinate = new Vector2(pos.x/(mult*sectors),pos.y/(mult*sectors));
	    newRipple.radius = RIPPLE_DEFAULT_RADIUS;
	    newRipple.strength = strength;
	    newRipple.runtime = radius*RIPPLE_DEFAULT_LIFESPAN/RIPPLE_DEFAULT_RADIUS;
	    newRipple.currentRadius = radius;
	    newRipple.rippleCycle = RIPPLE_DEFAULT_RIPPLE_CYCLE;
	    newRipple.lifespan = RIPPLE_DEFAULT_LIFESPAN;
	
	    // add ripple to running list
		m_rippleList.Add(newRipple);	
	}

	
	void initMesh() {
			
		MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
		
		meshFilter.sharedMesh = new Mesh();
		m_mesh = meshFilter.sharedMesh;
		
		m_vertices = new Vector3[(sectors+1)*(sectors+1)];
		m_edgeVertice = new bool[(sectors+1)*(sectors+1)];
		m_triangles = new int [2*sectors*sectors*3];
		
		m_uvs = new Vector2[(sectors+1)*(sectors+1)];
		m_uv_copy = new Vector2[(sectors+1)*(sectors+1)];
		
		for(int i=0;i<sectors+1;i++)
		{
			for (int j=0;j<sectors+1;j++)
			{
				//Debug.Log("Number: " +(i*(sectors+1)+j)+ " Vector " +new Vector3(j*10,i*10,0));
				m_vertices[i*(sectors+1)+j] = new Vector3(j*mult,i*mult,0);
				m_uvs[i*(sectors+1)+j] = new Vector2(((float)j)/sectors,((float)i)/sectors);
				m_uv_copy[i*(sectors+1)+j] = new Vector2(((float)j)/sectors,((float)i)/sectors);
				
				if (j == 0 || i == 0 || j==sectors || i==sectors)
					m_edgeVertice[i*(sectors+1)+j]=true;
				else
					m_edgeVertice[i*(sectors+1)+j]=false;
			}
		}
		
		for (int j =0;j<sectors;j++)
		{
			for(int i =0; i<sectors;i++)
			{
					m_triangles[3*2*(j*sectors+i)+2] = j*(sectors+1)+i;
					//Debug.Log(j*(sectors+1)+i);
					m_triangles[3*2*(j*sectors+i)+1] = j*(sectors+1)+i+1;
					//Debug.Log(j*(sectors+1)+i+1);
					m_triangles[3*2*(j*sectors+i)+0] = (j+1)*(sectors+1)+i;
					//Debug.Log((j+1)*(sectors+1)+i);
		
					m_triangles[3*2*(j*sectors+i)+5] = j*(sectors+1)+i+1;
					//Debug.Log(j*(sectors+1)+i+1);
					m_triangles[3*2*(j*sectors+i)+4] = (j+1)*(sectors+1)+i+1;
					//Debug.Log((j+1)*(sectors+1)+i+1);
					m_triangles[3*2*(j*sectors+i)+3] = (j+1)*(sectors+1)+i;
					//Debug.Log((j+1)*(sectors+1)+i);
				
			}
		}
		
		
		m_bufferSize = (sectors+1)*(sectors+1);
		
		m_mesh.vertices = m_vertices;
		m_mesh.uv = m_uvs;
		m_mesh.uv1 = m_uv_copy;
		m_mesh.uv2 = m_uv_copy;
		m_mesh.triangles = m_triangles;
			
		//m_mesh.RecalculateNormals();
		//m_mesh.Optimize();
	}
	
	void Update() {
   		
		rippleData ripple;
    	Vector2 pos;
    	float distance, correction;

    	// test if any ripples at all
    	if ( m_rippleList.Count == 0 ) return;
		
	 // ripples are simulated by altering texture coordinates
    // on all updates, an entire new array is calculated from the base array
    // not maintainng an original set of texture coordinates, could result in accumulated errors
    	m_uv_copy.CopyTo(m_uvs,0);

	    // scan through running ripples
	    // the scan is backwards, so that ripples can be removed on the fly
	    for ( int count = ( m_rippleList.Count - 1 ); count >= 0; count -- ) {
	
	        // get ripple data
	        ripple = m_rippleList[count];
	
			foreach(WaveDisturb waterD in disturb)
			{
				if (ripple.runtime <= ripple.lifespan/2)
				{
					if ((waterD.pos-ripple.center).magnitude < ripple.currentRadius + waterD.radius )
					{
						waterD.inWave = true;
					}
				}
			}
			
	        // scan through all texture coordinates
	        for ( int count1 = 0; count1 < m_bufferSize; count1 ++ ) {
	
	            // dont modify edge vertices
	            if ( m_edgeVertice[ count1 ] == false ) {
	
	                // calculate distance
	                // you might think it would be faster to do a box check first
	                // but it really isnt,
	                // ccpDistance is like my sexlife - BAM! - and its all over
	                distance = Vector3.Distance( ripple.center, m_vertices[ count1 ]);
	
	                // only modify vertices within range
	                if ( distance <= ripple.currentRadius ) {
	
	                    // load the texture coordinate into an easy to use var
	                    pos = m_uvs[ count1 ];  
	
	                    // calculate a ripple
	                    switch ( ripple.rippleType ) {
	
	                        case RIPPLE_TYPE.RIPPLE_TYPE_RUBBER:
	                            // method A
	                            // calculate a sinus, based only on time
	                            // this will make the ripples look like poking a soft rubber sheet, since sinus position is fixed
	                            correction = Mathf.Sin( 2 * Mathf.PI * ripple.runtime / ripple.rippleCycle );
	                            break;
	
	                        case RIPPLE_TYPE.RIPPLE_TYPE_GEL:
	                            // method B
	                            // calculate a sinus, based both on time and distance
	                            // this will look more like a high viscosity fluid, since sinus will travel with radius
	                            correction = Mathf.Sin( 2 * Mathf.PI * ( ripple.currentRadius - distance ) / ripple.radius * ripple.lifespan / ripple.rippleCycle );
	                            break;
	
	                        case RIPPLE_TYPE.RIPPLE_TYPE_WATER:
	                        default:
	                            // method c
	                            // like method b, but faded for time and distance to center
	                            // this will look more like a low viscosity fluid, like water     
	
	                            correction = ( ripple.radius * ripple.rippleCycle / ripple.lifespan ) / ( ripple.currentRadius - distance );
	                            if ( correction > 1.0f ) correction = 1.0f;
	
	                            // fade center of quicker
	                            correction *= correction;
	
	                            correction *= Mathf.Sin( 2 *Mathf.PI * ( ripple.currentRadius - distance ) / ripple.radius * ripple.lifespan / ripple.rippleCycle );
	                            break;
	
	                    }
	
	                    // fade with distance
	                    correction *= 1 - ( distance / ripple.currentRadius );
	
	                    // fade with time
	                    correction *= 1 - ( ripple.runtime / ripple.lifespan );
	
	                    // adjust for base gain and user strength
	                    correction *= RIPPLE_BASE_GAIN;
	                    correction *= ripple.strength;
	
	                    // finally modify the coordinate by interpolating
	                    // because of interpolation, adjustment for distance is needed,
	                    correction /= Vector2.Distance( ripple.centerCoordinate, pos );
	                    pos = pos + (pos- ripple.centerCoordinate)*correction;
	
	                    // another approach for applying correction, would be to calculate slope from center to pos
	                    // and then adjust based on this
	
	                    // clamp texture coordinates to avoid artifacts
	                    if (pos.x > 1) pos.x=1;
						if (pos.y > 1) pos.y=1;	
	
	                    // save modified coordinate
	                    m_uvs[ count1 ] = pos;
	
	                }
	            }
	        }
	
	        // calculate radius
	        ripple.currentRadius = ripple.radius * ripple.runtime / ripple.lifespan;
	
	        // check if ripple should expire
	        ripple.runtime += Time.deltaTime;
	        if ( ripple.runtime >= ripple.lifespan ) {
	
	            // free memory, and remove from list
	            m_rippleList.RemoveAt(count);
	
	        }
	
	    }
		
		m_mesh.uv = m_uvs;
	}
	
	public void addDistorsion(Vector2 vec,float str)
	{
		addRipple(vec + new Vector2(0,0+collibrate),RIPPLE_TYPE.RIPPLE_TYPE_WATER,str);
	}
	
	public void addDistorsion(Vector2 vec,float str, float radius)
	{
		addRipple(vec + new Vector2(0,0+collibrate),RIPPLE_TYPE.RIPPLE_TYPE_WATER,str,radius);
	}
	
	public void buttonAll() {
		addRipple(new Vector2(Input.mousePosition.x,Input.mousePosition.y+collibrate),RIPPLE_TYPE.RIPPLE_TYPE_WATER,0.2f);
	}

}
