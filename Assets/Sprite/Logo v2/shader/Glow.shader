Shader "UI/GlossyButton"
{
    Properties
    {
        _TopColor ("Top Color", Color) = (0.3,0.8,0.3,1)
        _BottomColor ("Bottom Color", Color) = (0.1,0.5,0.1,1)
        _Gloss ("Gloss Strength", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _TopColor;
            float4 _BottomColor;
            float _Gloss;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float gradient = i.uv.y;

                float4 baseColor = lerp(_BottomColor, _TopColor, gradient);

                float gloss = smoothstep(0.6, 1.0, gradient) * _Gloss;

                baseColor.rgb += gloss;

                return baseColor;
            }
            ENDCG
        }
    }
}