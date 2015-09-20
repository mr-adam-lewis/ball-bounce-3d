Shader "Gradient/VerticalLinearGradient" {
	Properties {
		_ColorTop ("Top Color", Color) = (1,1,1,1)
		_ColorBottom ("Bottom Color", Color) = (0,0,0,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		Pass {
		
			CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"
            
            uniform fixed4 _ColorTop;
            uniform fixed4 _ColorBottom;
            
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
			            	
            	float ratio = v.uv.y;
            	
            	fixed4 color = lerp (_ColorBottom, _ColorTop, ratio);
                return color;
            }
			
			ENDCG
		}
	} 
	FallBack "Unlit/Color"
}
