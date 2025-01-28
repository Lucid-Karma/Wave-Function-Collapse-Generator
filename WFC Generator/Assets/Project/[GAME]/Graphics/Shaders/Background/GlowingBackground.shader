Shader "Custom/GlowingBackground"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Main Color", Color) = (1, 1, 1, 1)
        _GlowColor ("Glow Color", Color) = (1, 1, 1, 1)
        _ScrollSpeed ("Scroll Speed", float) = 1.0
        _GlowIntensity ("Glow Intensity", float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // Uniforms
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _GlowColor;
            float _ScrollSpeed;
            float _GlowIntensity;

            // Vertex function
            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            // Fragment function
            half4 frag (Varyings IN) : SV_Target
            {
                // Scroll the texture
                float time = _Time.y * _ScrollSpeed;
                float2 scrolledUV = float2(IN.uv.x, IN.uv.y + time);

                // Sample the texture
                half4 texColor = tex2D(_MainTex, scrolledUV);
                half4 finalColor = texColor * _Color;

                // Add glow effect
                float glow = sin(_Time.y * _ScrollSpeed) * _GlowIntensity;
                half4 glowColor = _GlowColor * glow;

                // Combine base color and glow
                return finalColor + glowColor;
            }
            ENDHLSL
        }
    }
    FallBack "Unlit"
}
