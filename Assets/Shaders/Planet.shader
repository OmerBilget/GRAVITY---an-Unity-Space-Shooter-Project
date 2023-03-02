Shader "Custom/Planet"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _MetallicSmoothness("MetallicSmoothness(RGB)", 2D) = "white" {}
        _Normal("Normal",2D)="white" {}
        _LightIntensity("LightIntensity",Range(0,100)) =1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
            #pragma surface surf Directional
             #include "UnityPBSLighting.cginc"
      
        float _LightIntensity;
        fixed4 _Color;

        // Physically based Standard lighting model, and enable shadows on all light types
        
        half4 LightingDirectional(SurfaceOutputStandard s,half3 lightDir,half3 atten) {
            half nDot = max(0,dot(s.Normal,lightDir));
            half4 c;
            c.rgb = s.Albedo * _LightColor0.rgb * nDot * _LightIntensity;
            c.a = s.Alpha;
            return c;
}
        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _MetallicSmoothness;
        sampler2D _Normal;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_MetallicSmoothness;
            float2 uv_Normal;
            
        };

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            fixed4 m = tex2D(_MetallicSmoothness, IN.uv_MainTex) * _Color;
            fixed4 n = tex2D(_Normal, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = m.r;
            o.Smoothness = m.a;
            o.Normal = n.xyz;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
