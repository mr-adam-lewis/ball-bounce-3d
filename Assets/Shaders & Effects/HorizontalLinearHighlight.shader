Shader "Gradient/HorizontalLinearHighlight" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		[MaterialToggle] _Inverted ("Right To Left", Float) = 0
		_Intensity ("Intensity",  Range (0,1)) = 0.1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		Pass {
		
			CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"
            
            uniform fixed4 _Color;
            uniform float _Intensity;
			uniform float _Inverted;
            
            struct v2f {
            	float4 pos : SV_POSITION;
            	float2 uv : TEXCOORD0;
            };
            
            v2f vert (appdata_tan v) {
            	v2f o;
            	o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
            	o.uv = v.texcoord.xy;
            	return o;
            }

            fixed4 frag(v2f v) : COLOR {
            	
            	fixed4 inner = lerp (_Color, fixed4 (1,1,1,1), _Intensity);
            	
            	float ratio = v.uv.x;
				
				if (_Inverted != 0)
					ratio = 1 - ratio;
            	
            	fixed4 color = lerp (inner, _Color, ratio);
                return color;
            }
			
			ENDCG
		}
	} 
	FallBack "Unlit/Color"
}
