Shader "Custom/UI/Fade/SceneChangeFadeShader"
{
    Properties
    {
        // 画面分割数保持
        _DivideScreen("Divede Screen",vector) = (0 ,0, 0, 0)

        // アニメーション時間保持
        _AnimationTime("Animation Time",Range(0,30)) = 0
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent"}
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
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;

            float _AspectX;
            float _AspectY;
            int _SplitSize;

            float4 _MainColor;

            vector _DivideScreen;
            float _AnimationTime;

            float slide(float2 uvFloat,int2 uvInt,float time)
            {
                return step(time + 2.0 , uvFloat.x - (uvInt.x - time) + uvFloat.y - (uvInt.y - time));
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 uvFloat = float2(frac(i.uv.x * _DivideScreen.x),frac(i.uv.y * _DivideScreen.y));
                int2 uvInt = int2(floor(i.uv.x * _DivideScreen.x),floor(i.uv.y * _DivideScreen.y));

                float alpha = step(_AnimationTime,uvFloat.x + uvFloat.y);

                return float4(0,0,0, slide(uvFloat,uvInt,_AnimationTime));
            }
            ENDCG
        }
    }
}
