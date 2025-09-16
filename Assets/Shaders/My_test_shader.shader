Shader "Custom/SpriteOutline"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineSize ("Outline Size", Float) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "PreviewType"="Plane" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float4 vertex : SV_POSITION; float2 uv : TEXCOORD0; };

            sampler2D _MainTex;
            float4 _OutlineColor;
            float _OutlineSize;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);

                // simple 8-direction outline check
                float alpha = col.a;
                float outline = 0;
                float2 offsets[8] = {
                    float2(-_OutlineSize,0), float2(_OutlineSize,0),
                    float2(0,-_OutlineSize), float2(0,_OutlineSize),
                    float2(-_OutlineSize,-_OutlineSize), float2(-_OutlineSize,_OutlineSize),
                    float2(_OutlineSize,-_OutlineSize), float2(_OutlineSize,_OutlineSize)
                };
                for (int j=0;j<8;j++)
                    outline = max(outline, tex2D(_MainTex, i.uv + offsets[j]).a);

                if (alpha < 0.1 && outline > 0.1)
                    return _OutlineColor;
                else
                    return col;
            }
            ENDCG
        }
    }
}
