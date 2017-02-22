Shader "Time of Day/Halo"
{
    Properties
    {
        _Color   ("Main Color", Color) = (1,1,1,1)
        _Dropoff ("Dropoff",    float) = 1
        _Extrude ("Extrude",    float) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent-440"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }

        Fog
        {
            Mode Off
        }

        Pass
        {
            Cull Back
            ZWrite Off
            ZTest LEqual
            Blend One One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _Color;
            float _Dropoff;
            float _Extrude;

            struct v2f {
                float4 position : POSITION;
                fixed3 viewdir  : TEXCOORD0;
                fixed3 normal   : TEXCOORD1;
            };

            v2f vert(appdata_base v) {
                v2f o;

                o.position = mul(UNITY_MATRIX_MVP, v.vertex + float4(_Extrude*v.normal, 0));
                o.viewdir = ObjSpaceViewDir(v.vertex);
                o.normal = v.normal;

                return o;
            }

            fixed4 frag(v2f i) : COLOR {
                float4 shading = max(0, dot(normalize(i.normal), normalize(i.viewdir)));

                return _Color.a * _Color * pow(shading, _Dropoff);
            }

            ENDCG
        }
    }

    Fallback "None"
}
