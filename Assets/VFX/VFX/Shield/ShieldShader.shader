Shader "Unlit/ShieldShader"
{
    Properties
    {
        _BaseColor("Base Color", color) = (1,1,1,1)
        _FresnelColor("Fresnel Color", color) = (1,1,1,1)
        _FresnelPower("Fresnel Power", float) = 1
        _FresnelStrength("Fresnel Strength", float) = 1
        _Extrusion("Extrusion", float) = 1
        _NoiseSpeed("Noise Speed", float) = 1
        
        _Noise ("Noise", 2D) = "white" {}
        
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            //Blend SrcAlpha OneMinusSrcAlpha
            Blend One One
            Zwrite On ZTest LEqual Cull back

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 fresnel : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            fixed4 _BaseColor;
            fixed4 _FresnelColor;
            sampler2D _Noise;
            float4 _Noise_ST;
            float _Extrusion;
            float _FresnelPower;
            float _FresnelStrength;

            float _NoiseSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                float3 posWorld = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 normWorld = normalize(mul(unity_ObjectToWorld, v.normal));

                float3 I = normalize(posWorld - _WorldSpaceCameraPos.xyz);
                o.fresnel = pow(1.0 + dot(I, normWorld), _FresnelPower) * _FresnelStrength;

                v.vertex.xyz += v.normal * _Extrusion;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _Noise);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv1 = i.uv;
                uv1.y += _Time.y * _NoiseSpeed;
                fixed noise1 = tex2D(_Noise, uv1).r;

                float2 uv2 = i.uv;
                uv2.y -= _Time.y * _NoiseSpeed;
                fixed noise2 = tex2D(_Noise, uv2).g;

                float2 uv3 = i.uv;
                uv3.y += _Time.y * 1.2 * _NoiseSpeed;
                fixed noise3 = tex2D(_Noise, uv3).b;

                fixed noise = noise1 * noise2 * noise3;

                fixed4 final = _BaseColor;
                final.rgb += _FresnelColor * i.fresnel;
                return final * noise;
            }
            ENDCG
        }
    }
}
