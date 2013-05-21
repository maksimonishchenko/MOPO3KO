Shader "2D Art for Iphone Water" {
	
	Properties {_MainTex ("Texture", 2D) = ""}
	
	SubShader {
		Ztest Always
	   	Zwrite Off
	    Pass {
	        Fog { Mode Off }
			CGPROGRAM
// Upgrade NOTE: excluded shader from Xbox360; has structs without semantics (struct v2f members uv,uv1)
#pragma exclude_renderers xbox360
			#pragma vertex vert
			#pragma fragment frag
						
			
			sampler2D _MainTex;
			
			// vertex input: position, UV
			struct appdata {
			    float4 vertex : POSITION;
			    float2 texcoord : TEXCOORD0;
			    float2 texcoord1 : TEXCOORD1;
			};
			
			struct v2f {
			    float4 pos : SV_POSITION;
			    float2 uv : TEXCOORD0;
			    float2 uv1 : TEXCOORD1;
			};
			
			v2f vert (appdata v) {
			    v2f o;
			    o.pos = mul( UNITY_MATRIX_MVP, v.vertex );
			    o.uv = v.texcoord.xy;
			    o.uv1 = v.texcoord1.xy;
			    
			    return o;
			}
			
			half4 frag( v2f i ) : COLOR {
			    half4 texcol = tex2D (_MainTex, i.uv);
			       			
    			float d = 1+10f*distance(i.uv,i.uv1);
    			
    			return texcol*d;
			}
			
			ENDCG
	    }
	}
}