Shader "UI/LevelBackgroundSprite_Directional"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Main Color", Color) = (1, 1, 1, 1)
        _GlowColor ("Glow Color", Color) = (1, 1, 1, 1)
        _ScrollSpeed ("Scroll Speed", float) = 1.0
        _GlowIntensity ("Glow Intensity", float) = 1.0
        _GlowTargetColor ("Glow Target Color", Color) = (1, 0, 0, 1) // Default is red
        _GlowThreshold ("Glow Threshold", float) = 0.1 // How closely the pixel needs to match the target color
        _ScrollDirection ("Scroll Direction (X, Y)", Vector) = (1, 0, 0, 0) // Default: horizontal
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

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
            float4 _GlowTargetColor; // Color to target for glowing
            float _GlowThreshold; // Threshold to control how close the color needs to be
            float4 _ScrollDirection; // Scroll direction

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
                // Calculate scrolling based on the direction
                float time = _Time.y * _ScrollSpeed;
                float2 scrolledUV = IN.uv + (_ScrollDirection.xy * time); // Scroll in the defined direction

                // Sample the texture
                half4 texColor = tex2D(_MainTex, scrolledUV);
                half4 finalColor = texColor * _Color;

                // Calculate glow based on color match
                float colorDifference = distance(texColor.rgb, _GlowTargetColor.rgb);
                float glowFactor = step(colorDifference, _GlowThreshold); // 1 if color is within threshold, 0 otherwise

                // Apply glow intensity only to the pixels matching the target color
                float glow = sin(_Time.y * _ScrollSpeed) * _GlowIntensity * glowFactor;
                half4 glowColor = _GlowColor * glow;

                // Combine base color and glow
                return finalColor + glowColor;
            }
            ENDHLSL
        }
    }
    FallBack "UI/Default"
}
