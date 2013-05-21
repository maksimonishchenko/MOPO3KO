Shader "2D Art for iPhone Colored" {
   
	Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Texture  (A = Transparency)", 2D) = ""}
	
	SubShader {   
	   Tags {Queue = Transparent}
	   Ztest Always
	   Zwrite Off
	   Blend SrcAlpha OneMinusSrcAlpha
	   Pass {
	   	SetTexture[_MainTex] {
	   		constantColor [_Color]
	   		Combine texture*constant
	   	}
	   }
	}

}