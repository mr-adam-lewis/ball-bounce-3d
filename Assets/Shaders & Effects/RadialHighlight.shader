Shader "Gradient/RadialHighlight" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Intensity ("Intensity",  Range (0,1)) = 0.1
		_CenterPositionX ("Center X Position", Range (0,1)) = 0.5
		_CenterPositionY ("Center Y Position", Range (0,1)) = 0.5
		_Radius ("Radius", Range (0.01, 1)) = 0.3
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
            uniform float _CenterPositionX;
            uniform float _CenterPositionY;
            uniform float _Radius;
            
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
				fixed2 diff = fixed2 (v.uv.x - _CenterPositionX, v.uv.y - _CenterPositionY);
            	float dist = length (diff);
            	
            	fixed4 inner = lerp (_Color, fixed4 (1,1,1,1), _Intensity);
            	
            	float ratio = dist / _Radius;
            	if (ratio > 1)
            		ratio = 1;
            	
            	fixed4 color = lerp (inner, _Color, ratio);
                return color;
            }
			
			ENDCG
		}
	} 
	FallBack "Unlit/Color"
}
