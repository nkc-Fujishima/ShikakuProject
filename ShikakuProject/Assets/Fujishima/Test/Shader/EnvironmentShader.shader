Shader "Custom/EnvironmentShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PlayerColor("Player Silhouette Color",Color)=(.0,.0,.0,1)
        _EnemyColor("Player Silhouette Color",Color)=(.0,.0,.0,1)

        _DarkFactor("Dark Factor",Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" 
               // "Queue"="Transparent"
           }
             
        LOD 100

        //--------------------------------------------------------
        // 裏のキャラクターが居ない場合の描画
        Pass
        {
            Stencil
            {
                Ref 0
                Comp Equal
            }
            

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }

        //--------------------------------------------------------
        // 裏にプレイヤーが存在する場合の描画
        Pass
        {
            Stencil
            {
                Ref 1
                Comp Equal
            }
            CGPROGRAM

                        #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            uniform float4 _PlayerColor;
            fixed4 frag (v2f i) : SV_Target
            {
                return _PlayerColor;
            }
            ENDCG
        }

        //--------------------------------------------------------
        // 裏にエネミーが存在する場合の描画
        Pass
        {
            Stencil
            {
                Ref 2
                Comp Equal
            }
            CGPROGRAM

                        #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            uniform float4 _EnemyColor;
            fixed4 frag (v2f i) : SV_Target
            {
                return _EnemyColor;
            }
            ENDCG
        }
    }

    FallBack "Standard"
}
