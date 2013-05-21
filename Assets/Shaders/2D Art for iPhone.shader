Shader "2D Art for iPhone" {
   
	Properties {_MainTex ("Texture  (A = Transparency)", 2D) = ""}
	
	SubShader {   
	   Tags {Queue = Transparent}
	   Ztest Always
	   Zwrite Off
	   Blend SrcAlpha OneMinusSrcAlpha
	   Pass {SetTexture[_MainTex]}
	}

}