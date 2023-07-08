Shader "Hidden/Custom/OverlayGradient"
{
	Properties 
	{
	    _MainTex ("Main Texture", 2D) = "white" {}
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
		
		Pass
		{
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            
			#pragma vertex vert
			#pragma fragment frag
			
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
			float _Radius;
            float _Feather;

            float4 _tlColor;
            float4 _trColor;
            float4 _blColor;
            float4 _brColor;
            
            struct Attributes
            {
                float4 positionOS       : POSITION;
                float2 uv               : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv        : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            
            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.vertex = vertexInput.positionCS;
                output.uv = input.uv;
                
                return output;
            }

            float4 getColor(float4 col, float2 origin, float2 uv)
            {
	            float dist = length(origin - uv);
            	float step = (1 - smoothstep(_Radius - _Feather, _Radius, dist));
            	return col * step;
            }

            float4 blend(float4 base, float4 layer)
            {
	            return 1 - (1 - layer) * (1 - base);
            }
            
            float4 frag (Varyings input) : SV_Target 
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
            	
            	// return color + getColor(_tlColor, float2(0, 1), input.uv) + getColor(_trColor, float2(1, 1), input.uv) +
            	// 	getColor(_blColor, float2(0, 0), input.uv) + getColor(_brColor, float2(1, 0), input.uv);
            	return 1 - (1 - color) * (1 - getColor(_tlColor, float2(0, 1), input.uv)) * (1 - getColor(_trColor, float2(1, 1), input.uv)) *
            		(1 - getColor(_blColor, float2(0, 0), input.uv)) * (1 - getColor(_brColor, float2(1, 0), input.uv));
            }
            
			ENDHLSL
		}
	} 
	FallBack "Diffuse"
}