// This is a custom shader that allows changing the transparency and color of an unlit texture
Shader "Custom/UnlitTransparent"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Alpha("Alpha", Range(0,1)) = 1
        _Color("Color", Color) = (1,1,1,1)
    }
        SubShader
        {
            Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
            LOD 100

            Blend SrcAlpha OneMinusSrcAlpha

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    UNITY_FOG_COORDS(1)
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float _Alpha;
                float4 _Color;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    UNITY_TRANSFER_FOG(o,o.vertex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    
                    fixed4 col = tex2D(_MainTex, i.uv);
                
                UNITY_APPLY_FOG(i.fogCoord, col);
                
                col *= _Alpha;
                
                col *= _Color;
                return col;
            }
            ENDCG
        }
        }
}
