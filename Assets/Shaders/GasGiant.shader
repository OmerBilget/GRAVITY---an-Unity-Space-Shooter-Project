// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/GasGiant"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Distort_Iteration("distort_Iteration",int) = 5
        _TIME_SCALE("time_scale", float) = 1
        _TEX_SCALE("tex_scale", float) = 1
        _LightDir("lightDir",Vector) = (1,0,0,0)
        _RimColor("Rim Color", Color) = (0.26,0.19,0.16,0.0)
        _RimPower("Rim Power", Range(0.01,8.0)) = 3.0
    }
        SubShader
    {
        Tags {"RenderType" = "Opaque" }

      

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            int _Distort_Iteration;
            float  _TIME_SCALE;
            float _TEX_SCALE;
            float3 _LightDir;
            float4 _RimColor;
            float _RimPower;
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;

            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 viewDir : TEXCOORD1;
                float3 normal :TEXCOORD2;
                float3 worldPos:TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _Channel0;
            float _TIMER;

          


            float hash(float n) { return frac(sin(n) * 123.456789); }

            float2 rotate(float2 uv, float a)
            {
                float c = cos(a);
                float s = sin(a);
                return float2(c * uv.x - s * uv.y, s * uv.x + c * uv.y);
            }

            float noise(float3 p)
            {
                float3 fl = floor(p);
                float3 fr = frac(p);
                fr = fr * fr * (3.0 - 2.0 * fr);

                float n = fl.x + fl.y * 157.0 + 113.0 * fl.z;
                return lerp(lerp(lerp(hash(n + 0.0), hash(n + 1.0), fr.x),
                    lerp(hash(n + 157.0), hash(n + 158.0), fr.x), fr.y),
                    lerp(lerp(hash(n + 113.0), hash(n + 114.0), fr.x),
                        lerp(hash(n + 270.0), hash(n + 271.0), fr.x), fr.y), fr.z);
            }

            float fbm(float2 p, float t)
            {
                float f;
                f = 0.5000 * noise(float3(p, t)); p *= 2.1;
                f += 0.2500 * noise(float3(p, t)); p *= 2.2;
                f += 0.1250 * noise(float3(p, t)); p *= 2.3;
                f += 0.0625 * noise(float3(p, t));
                return f;
            }

            float2 map(in float2 uv)
            {
                uv.x *= 5.0;
                uv.x +=  _Time ;
                uv.y *= 15.0;
                return uv;
            }


            float2 field(in float2 p)
            {
                float t =_TIME_SCALE* _Time ;

                p.x += t;

                float n = fbm(p, t);

                float e = 0.25;
                float nx = fbm(p + float2(e, 0.0), t);
                float ny = fbm(p + float2(0.0, e), t);

                return float2(n - ny, nx - n) / e;
            }


            float3 distort(float2 p)
            {
                for (float i = 0.0; i < 5.0; ++i)
                {
                    p += field(p) / 5.0;
                }
                float3 s = 2.5 * tex2D(_MainTex, float2(0.0, p.y)*_TEX_SCALE).xyz;

                return fbm(p, 0.0) * s;
            }

            float3 doMaterial(float3 pos,float2 uv)
            {
                uv.x = atan2(pos.x, pos.z);
                uv.y = asin(pos.y);
                return distort(map(uv));
            }
           

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.viewDir= WorldSpaceViewDir(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;


            }



            fixed4 frag (v2f i) : SV_Target
            {
              
               
        
                half r = 1- saturate(dot(normalize(i.viewDir),i.normal));
                float2 uv = i.uv-0.5;//i.vertex.xy/_ScreenParams.xy-0.5;
                float3 LightDir =normalize( _WorldSpaceLightPos0 - i.worldPos);
                float light = max(0, dot(LightDir, i.normal));
                uv.x *= _ScreenParams.x / _ScreenParams.y;
                float alpha;
                float p = 1 - pow(r , _RimPower);
                if (r >= 0.99) {
                    p = 0;
                   
                }
                else {       
                    alpha = 1;
                }
                if (p < 0.0001) {
                }
                float3 c2 = float3(distort(map(uv)))*p*(1.4-r)*light*light*light;
                c2 = pow(c2, float3( 0.4545 , 0.4545, 0.4545));
                return float4(c2.x,c2.y,c2.z,alpha);
            }
            ENDCG
        }
    }
}
