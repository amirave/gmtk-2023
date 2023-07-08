Shader "Sketch"
{
    Properties {
        _CellSize ("Cell Size", Range(2, 50)) = 2
        _LinesThreshold ("Lines Threshold", Range(-1, 1)) = 0
        _LineFrequency ("Lines Threshold", Range(1, 50)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "ColorBlitPass"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            // The Blit.hlsl file provides the vertex shader (Vert),
            // input structure (Attributes) and output strucutre (Varyings)
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment frag

            TEXTURE2D_X(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);

            float2 rotate(float2 v, float angle)
            {
                return float2(v.x * cos(angle) - v.y * sin(angle), v.x * sin(angle) + v.y * cos(angle));
            }
            
            float2 rand2dTo2d(float2 v)
            {
                return float2(frac(sin(dot(v, float2(12.9898,78.233))) * 43758.5453123),
                              frac(sin(dot(v, float2(543.2134 ,2.2359876))) * 1244.3532224));
            }

            float2 rand2dTo1d(float2 v)
            {
                return frac(sin(dot(v, float2(12.9898,78.233))) * 23054.5453123);
            }

            float2 voronoiNoise(float2 value){
                float2 baseCell = floor(value);

                float minDistToCell = 10;
                float2 closestCell;
                [unroll]
                for(int x=-1; x<=1; x++){
                    [unroll]
                    for(int y=-1; y<=1; y++){
                        float2 cell = baseCell + float2(x, y);
                        float2 cellPosition = cell + rand2dTo2d(cell);
                        float2 toCell = cellPosition - value;
                        float distToCell = length(toCell);
                        if(distToCell < minDistToCell){
                            minDistToCell = distToCell;
                            closestCell = cell;
                        }
                    }
                }
                float random = rand2dTo1d(closestCell);
                return float2(minDistToCell, random);
            }

            float3 rgb2hcv(in float3 rgb)
            {
                // Based on work by Sam Hocevar and Emil Persson
                float4 P = lerp(float4(rgb.bg, -1.0, 2.0/3.0), float4(rgb.gb, 0.0, -1.0/3.0), step(rgb.b, rgb.g));
                float4 Q = lerp(float4(P.xyw, rgb.r), float4(rgb.r, P.yzx), step(P.x, rgb.r));
                float C = Q.x - min(Q.w, Q.y);
                float H = abs((Q.w - Q.y) / (6 * C + 0.00001) + Q.z);
                return float3(H, C, Q.x);
            }

            float3 hsl2rgb(float3 c)
            {
                c = float3(frac(c.x), clamp(c.yz, 0.0, 1.0));
                float3 rgb = clamp(abs(fmod(c.x * 6.0 + float3(0.0, 4.0, 2.0), 6.0) - 3.0) - 1.0, 0.0, 1.0);
                return c.z + c.y * (rgb - 0.5) * (1.0 - abs(2.0 * c.z - 1.0));
            }

            float BezierBlend(float t)
            {
                return t * t * (3.0f - 2.0f * t);
            }

            float _CellSize;
            float _LinesThreshold;
            float _LineFrequency;

            half4 frag (Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                float4 color = SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, input.texcoord);
                float lum = color.r * 0.3 + color.g * 0.59 + color.b * 0.11;
                float hue = rgb2hcv(color.xyz).x;
                float3 saturated = hsl2rgb(float3(hue, 1, 0.5));
                
                float2 value = input.positionCS / _CellSize;
                float cell = voronoiNoise(value).y;

                float angle = cell * 2 * PI;
                float waves = sin(rotate(value, angle) * _LineFrequency);
                waves = 0.5f * waves + 0.5f;
                float lines = step(waves, BezierBlend(lum));
                
                return lerp(color/lum, half4(1,1,1,1), lines);
            }
            ENDHLSL
        }
    }
}