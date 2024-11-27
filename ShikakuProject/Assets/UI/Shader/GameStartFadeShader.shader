Shader "Custom/UI/Fade/GameStartFadeShader"
{
    Properties
    {
        _ExpandStartPos("_Expand StartPos",vector) = (0,0,0,0)

        _AnimationTime("Animation Time",Range(0,1)) = 0
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

            vector _ExpandStartPos;
            float _AnimationTime;

            float Expand(float2 uv,float animationTime)
            {
                float size = 0.5 + animationTime * 0.5;

                float xValue = step(uv.x,size) * step(1.0 - uv.x,size); 
                float yValue = step(uv.y,size) * step(1.0 - uv.y,size);

                return xValue * yValue;
            }

            float TestExpand(float2 uv,float animationTime)
            {
                float distanceValueX = distance(_ExpandStartPos.x,uv.x);
                float distanceValueY = distance(_ExpandStartPos.y,uv.y);

                float convert = step(distanceValueX,_AnimationTime)*step(distanceValueY,_AnimationTime);

                return abs(convert - 1);
            }

            float4 frag (v2f i) : SV_Target
            {

                return float4(0,0,0,TestExpand(i.uv,_AnimationTime));

            }
            ENDCG
        }
    }
}
