Shader "Gradient/RadialGradient" {
	Properties {
		_CenterColor ("Center Color", Color) = (1,1,1,1)
		_OuterColor ("Outer Color", Color) = (0,0,0,1)
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
            
            uniform fixed4 _CenterColor;
			uniform fixed4 _OuterColor;
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
            	
            	float ratio = dist / _Radius;
            	if (ratio > 1)
            		ratio = 1;
            	
            	fixed4 color = lerp (_CenterColor, _OuterColor, ratio);
                return color;
            }
			
			ENDCG
		}
	} 
	FallBack "Unlit/Color"
}
