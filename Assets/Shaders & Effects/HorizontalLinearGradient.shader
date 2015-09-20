Shader "Gradient/HorizontalLinearGradient" {
	Properties {
		_ColorLeft ("Left Color", Color) = (1,1,1,1)
		_ColorRight ("Right Color", Color) = (0,0,0,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		Pass {
		
			CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"
            
            uniform fixed4 _ColorLeft;
            uniform fixed4 _ColorRight;
            
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
			            	
            	float ratio = v.uv.x;
            	
            	fixed4 color = lerp (_ColorLeft, _ColorRight, ratio);
                return color;
            }
			
			ENDCG
		}
	} 
	FallBack "Unlit/Color"
}
