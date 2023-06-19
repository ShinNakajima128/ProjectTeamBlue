// This is a custom shader that allows changing the transparency and color of an unlit texture
Shader "Custom/UnlitTransparent"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}// テクスチャを指定するプロパティ
        _Alpha("Alpha", Range(0,1)) = 1// 透明度を指定するプロパティ
        _Color("Color", Color) = (1,1,1,1)// 色を指定するプロパティ
    }
        SubShader
        {
            // 透明なオブジェクトのレンダリングに使用されるタグ(https://docs.unity3d.com/ja/2019.4/Manual/SL-SubShaderTags.html)
            Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
            /*LOD 100 は、シェーダーのLOD（Level of Detail）を指定するものです。
            LODは、シェーダーの複雑さを表す値で、値が大きいほど複雑なシェーダーを表します。
            Unityでは、カメラから遠く離れたオブジェクトに対して、より単純なシェーダーを使用することができます。
            これにより、パフォーマンスが向上します。LOD 100 は、このシェーダーが中程度の複雑さであることを示しています。*/
            LOD 100

            /*透明度が異なる複数のオブジェクトが重なった場合に、正しく表示されます。
            他にも様々なブレンディング設定があります。
            例えば、Zero、One、SrcColor、DstColor、SrcAlpha、DstAlpha、OneMinusSrcColor、OneMinusDstColorなどがあります。

            https://docs.unity3d.com/ja/2018.4/Manual/SL-Blend.html
            */
            Blend SrcAlpha OneMinusSrcAlpha

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata//頂点データを格納するために使用
                {
                    float4 vertex : POSITION;//頂点の位置
                    float2 uv : TEXCOORD0;//テクスチャ座標
                };

                struct v2f//頂点シェーダーからフラグメントシェーダーにデータを渡すために使用
                {
                    float2 uv : TEXCOORD0;//テクスチャ座標
                    UNITY_FOG_COORDS(1)//フォグ効果を適用するために必要
                    float4 vertex : SV_POSITION;//頂点の位置
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float _Alpha;
                float4 _Color;

                v2f vert(appdata v)//頂点シェーダーで、頂点データを受け取り、変換して返します
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);//頂点座標をオブジェクト空間からクリップ空間に変換します。
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);//テクスチャ座標を変換
                    UNITY_TRANSFER_FOG(o,o.vertex);//フォグ効果を適用するために必要
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target//フラグメントシェーダーで、ピクセルデータを受け取り、最終的なピクセルカラーを返します
                {
                    
                    fixed4 col = tex2D(_MainTex, i.uv);//テクスチャからピクセルカラーを取得します
                
                UNITY_APPLY_FOG(i.fogCoord, col);//フォグ効果を適用します
                
                //ピクセルカラーにアルファ値と色を乗算して返します
                col *= _Alpha;
                
                col *= _Color;
                return col;
            }
            ENDCG
        }
        }
}
