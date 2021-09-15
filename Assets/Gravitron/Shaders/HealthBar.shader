Shader "Unlit/HealthBar"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
        _BGColor("BackGroundColour", Color) = (1,1,1,1)
        
        [space]
        _Health("Health", range(0, 1)) = 1
        _FlashPos("Flash", range(0, 1)) = .13
        _BoardThickness("ThiccBoarder", range(0, 1)) = .5
        _Segments("Segement", int) = 8
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent"} 
        LOD 100

        Pass
        {
            cull off
            ZWrite off
            Blend SrcAlpha OneMinusSrcAlpha
            // src * X + dst * Y
            // src * SrcAlpha + dst * 1 - SrcAlpha 
            
            // src = this pixel we are rendering 
            // dst = the background pixel
            
            // src = red
            // src * 0.420
            
            // dst * (1 - 0.420) 0.580
            
            // pixel color = src + dst
            
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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _BGColor;

            float _Health;
            float _FlashPos;
            float _BoardThickness;
            int _Segments;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float InverseLerp(float a, float b, float v)
            {
                return (v-a)/(b-a);
            }
            float flash()
            {
                 return cos(_Time.y * 6) * 0.4 + 1;
            }
            
            // handles the boarder of the of
            // the health bar and cliping of it
            float BoarderSDF(v2f i)
            {
                float2 coords = i.uv;
                coords.x *= 8;

                const float2 point_on_line = float2(clamp(coords.x, .5, 7.5), .5);
                const float SDF = distance(coords, point_on_line) * 2 - 1;

                clip(-SDF);
                return SDF + _BoardThickness;
            }

            float Choppy(v2f i)
            {
                const float uvx = i.uv.x;
                return 1 - (distance(uvx, floor(uvx * _Segments + 1) / _Segments)< 0.005);
            }

            bool HealthBarMask(v2f i)
            {
                const float uvx = i.uv.x;
                return floor(uvx * _Segments) / _Segments < _Health;
            }

            float3 HealthBarColor(v2f i)
            {
                const float uvy = i.uv.y;
                
                return tex2D(_MainTex, float2(_Health, uvy))
                * lerp(1, flash(), _Health < _FlashPos); 
            }

            fixed4 OutPut(v2f i)
            {
                float pd = fwidth(BoarderSDF(i));
                float boarderMask = saturate(BoarderSDF(i) / pd);
                //float boarderMask = step(0, BoarderSDF(i));

                float healthFade = InverseLerp(0.999, 1, _Health);
                float3 outColor = lerp(_BGColor, HealthBarColor(i), HealthBarMask(i));

                return fixed4(outColor.xyz * boarderMask, saturate(lerp(0, 1, 1))) * fixed4(Choppy(i).xxx, 1);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return OutPut(i);
            }
            ENDCG
        }
    }
}
