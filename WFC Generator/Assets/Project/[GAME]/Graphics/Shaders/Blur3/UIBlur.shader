Shader "Custom/UIBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // Declare the main texture
        _BlurAmount ("Blur Amount", Range(0, 10)) = 1 // Blur strength
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex; // Declare the texture sampler
            float4 _MainTex_ST; // Texture scale and offset
            float _BlurAmount;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex); // Apply texture scale and offset
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                fixed4 col = tex2D(_MainTex, uv); // Sample the main texture
                float blur = _BlurAmount / 100.0;

                // Apply blur by sampling surrounding pixels
                col += tex2D(_MainTex, uv + float2(blur, blur));
                col += tex2D(_MainTex, uv + float2(-blur, blur));
                col += tex2D(_MainTex, uv + float2(blur, -blur));
                col += tex2D(_MainTex, uv + float2(-blur, -blur));
                col /= 5.0; // Average the samples

                return col;
            }
            ENDCG
        }
    }
}