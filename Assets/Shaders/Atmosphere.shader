Shader "Custom/A"
{
    Properties
    {
         _Center("center",Vector) = (0,0,0,0)
         _LightDir("lightDir",Vector) = (1,0,0,0)
         _AtmosphereRadius("AtmosphereRad",Range(0,1000)) = 1
         _PlanetRadius("PlanetRad",Range(0,1000)) = 0.5
         _Sample("Sample",Range(0,160)) = 5
         _SampleLight("SampleLight",Range(0,160)) = 5
         _RayBeta("RayBeta",Vector) = (0.0000055, 0.0000013, 0.00000224)
         _MieBeta("MieBeta" ,Vector) = (0.0000021,0,0)
         _AmbientBeta("AmbientBeta",Vector) = (0,0,0)
         _AbsorptionBeta("AbsorbtionBeta",Vector) = (0.00002, 0.00005, 0.000002)
         _G("G",Range(-1,1)) = 0.7
         _HeightRay("HeightRay",Range(0,50000)) = 0.00008
         _HeightMie("HeightMie",Range(0,50000)) = 0.00008
         _HeightAbsorbtion("HeightAbsorb",Range(0,50000)) = 0.00008
         _AbsorbtionFalloff("AbsorbtionFalloff",Range(0,50000)) = 0.00008
         _ViewRange("ViewDistance",Range(0,3000)) = 700
         _LightIntensity("LightIntensity",Vector) = (40,40,40,0)
    }
        SubShader
    {
        Tags { "Queue" = "Transparent"}
      
    
       //Blend One One 
        Blend SrcAlpha SrcAlpha
       //Blend One DstColor, One One
       //Blend OneMinusDstColor One
        Cull Off //double side
        
        ZWrite  Off 
        //ZTest Off
        Lighting Off
        
        BlendOp Add
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work


            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float3 wPos : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            #define STEP 64;
            #define STEP_SIZE 0.01

            float4 _Center;
            float _Dist;
            float4 _LightDir;
            float _AtmosphereRadius;
            float _PlanetRadius;
            int _Sample;
            int _SampleLight;
            float4 _RayBeta;
            float4 _MieBeta;
            float4 _AmbientBeta;
            float4 _AbsorbtionBeta;
            float _G;
            float _HeightRay;
            float _HeightMie;
            float _HeightAbsorbtion;
            float _AbsorbtionFalloff;
            float _ViewRange;
            float3 _LightIntensity;
            bool SphereHit(float3 pos,float3 center, float radius) {
                return distance(pos, center) < radius;
            }



            float3 calculate_scattering(
                float3 start, 				// the start of the ray (the camera position)
                float3 dir, 					// the direction of the ray (the camera vector)
                float max_dist, 			// the maximum distance the ray can travel (because something is in the way, like an object)
                float3 scene_color,			// the color of the scene
                float3 light_dir, 			// the direction of the light
                float3 light_intensity,		// how bright the light is, affects the brightness of the atmosphere
                float3 planet_position, 		// the position of the planet
                float planet_radius, 		// the radius of the planet
                float atmo_radius, 			// the radius of the atmosphere
                float3 beta_ray, 				// the amount rayleigh scattering scatters the colors (for earth: causes the blue atmosphere)
                float3 beta_mie, 				// the amount mie scattering scatters colors
                float3 beta_absorption,   	// how much air is absorbed
                float3 beta_ambient,			// the amount of scattering that always occurs, cna help make the back side of the atmosphere a bit brighter
                float g, 					// the direction mie scatters the light in (like a cone). closer to -1 means more towards a single direction
                float height_ray, 			// how high do you have to go before there is no rayleigh scattering?
                float height_mie, 			// the same, but for mie
                float height_absorption,	// the height at which the most absorption happens
                float absorption_falloff,	// how fast the absorption falls off from the absorption height
                int steps_i, 				// the amount of steps along the 'primary' ray, more looks better but slower
                int steps_l 				// the amount of steps along the light ray, more looks better but slower
            ) {
                // add an offset to the camera position, so that the atmosphere is in the correct position
                start -= planet_position;
                // calculate the start and end position of the ray, as a distance along the ray
                // we do this with a ray sphere intersect
                float a = dot(dir, dir);
                float b = 2.0 * dot(dir, start);
                float c = dot(start, start) - (atmo_radius * atmo_radius);
                float d = (b * b) - 4.0 * a * c;

                // stop early if there is no intersect
                if (d < 0.0) return scene_color;

                // calculate the ray length
                float2 ray_length = float2(
                    max((-b - sqrt(d)) / (2.0 * a), 0.0),
                    min((-b + sqrt(d)) / (2.0 * a), max_dist)
                );

                // if the ray did not hit the atmosphere, return a black color
                if (ray_length.x > ray_length.y) return scene_color;
                // prevent the mie glow from appearing if there's an object in front of the camera
                bool allow_mie = max_dist > ray_length.y;
                // make sure the ray is no longer than allowed
                ray_length.y = min(ray_length.y, max_dist);
                ray_length.x = max(ray_length.x, 0.0);
                // get the step size of the ray
                float step_size_i = (ray_length.y - ray_length.x) / float(steps_i);

                // next, set how far we are along the ray, so we can calculate the position of the sample
                // if the camera is outside the atmosphere, the ray should start at the edge of the atmosphere
                // if it's inside, it should start at the position of the camera
                // the min statement makes sure of that
                float ray_pos_i = ray_length.x + step_size_i * 0.5;

                // these are the values we use to gather all the scattered light
                float3 total_ray = float3(0.0,0.0,0.0); // for rayleigh
                float3 total_mie = float3(0.0,0.0,0.0); // for mie

                // initialize the optical depth. This is used to calculate how much air was in the ray
                float3 opt_i = float3(0.0,0.0,0.0);

                // also init the scale height, avoids some vec2's later on
                float2 scale_height = float2(height_ray, height_mie);

                // Calculate the Rayleigh and Mie phases.
                // This is the color that will be scattered for this ray
                // mu, mumu and gg are used quite a lot in the calculation, so to speed it up, precalculate them
                float mu = dot(dir, light_dir);
                float mumu = mu * mu;
                float gg = g * g;
                float phase_ray = 3.0 / (50.2654824574 /* (16 * pi) */) * (1.0 + mumu);
                float phase_mie = allow_mie ? 3.0 / (25.1327412287 /* (8 * pi) */) * ((1.0 - gg) * (mumu + 1.0)) / (pow(1.0 + gg - 2.0 * mu * g, 1.5) * (2.0 + gg)) : 0.0;
                float3 pos_i;
                float3 height_i;
                float3 density;
                // now we need to sample the 'primary' ray. this ray gathers the light that gets scattered onto it
                for (int i = 0; i < steps_i; ++i) {

                    // calculate where we are along this ray
                    pos_i = start + dir * ray_pos_i;

                    // and how high we are above the surface
                    height_i = length(pos_i) - planet_radius;

                    // now calculate the density of the particles (both for rayleigh and mie)
                    density = float3(exp(-height_i / scale_height), 0.0);

                    // and the absorption density. this is for ozone, which scales together with the rayleigh, 
                    // but absorbs the most at a specific height, so use the sech function for a nice curve falloff for this height
                    // clamp it to avoid it going out of bounds. This prevents weird black spheres on the night side
                    float denom = (height_absorption - height_i) / absorption_falloff;
                    density.z = (1.0 / (denom * denom + 1.0)) * density.x;

                    // multiply it by the step size here
                    // we are going to use the density later on as well
                    density *= step_size_i;

                    // Add these densities to the optical depth, so that we know how many particles are on this ray.
                    opt_i += density;

                    // Calculate the step size of the light ray.
                    // again with a ray sphere intersect
                    // a, b, c and d are already defined
                    a = dot(light_dir, light_dir);
                    b = 2.0 * dot(light_dir, pos_i);
                    c = dot(pos_i, pos_i) - (atmo_radius * atmo_radius);
                    d = (b * b) - 4.0 * a * c;

                    // no early stopping, this one should always be inside the atmosphere
                    // calculate the ray length
                    float step_size_l = (-b + sqrt(d)) / (2.0 * a * float(steps_l));

                    // and the position along this ray
                    // this time we are sure the ray is in the atmosphere, so set it to 0
                    float ray_pos_l = step_size_l * 0.5;

                    // and the optical depth of this ray
                    float3 opt_l = float3(0.0,0.0,0.0);

                    // now sample the light ray
                    // this is similar to what we did before
                    for (int l = 0; l < steps_l; ++l) {

                        // calculate where we are along this ray
                        float3 pos_l = pos_i + light_dir * ray_pos_l;

                        // the heigth of the position
                        float height_l = length(pos_l) - planet_radius;

                        // calculate the particle density, and add it
                        // this is a bit verbose
                        // first, set the density for ray and mie
                        float3 density_l = float3(exp(-height_l / scale_height), 0.0);

                        // then, the absorption
                        float denom = (height_absorption - height_l) / absorption_falloff;
                        density_l.z = (1.0 / (denom * denom + 1.0)) * density_l.x;

                        // multiply the density by the step size
                        density_l *= step_size_l;

                        // and add it to the total optical depth
                        opt_l += density_l;

                        // and increment where we are along the light ray.
                        ray_pos_l += step_size_l;

                    }

                    // Now we need to calculate the attenuation
                    // this is essentially how much light reaches the current sample point due to scattering
                    float3 attn = exp(-beta_ray * (opt_i.x + opt_l.x) - beta_mie * (opt_i.y + opt_l.y) - beta_absorption * (opt_i.z + opt_l.z));

                    // accumulate the scattered light (how much will be scattered towards the camera)
                    total_ray += density.x * attn;
                    total_mie += density.y * attn;

                    // and increment the position on this ray
                    ray_pos_i += step_size_i;

                }

                // calculate how much light can pass through the atmosphere
                float3 opacity = exp(-(beta_mie * opt_i.y + beta_ray * opt_i.x + beta_absorption * opt_i.z));

                // calculate and return the final color
                return (
                    phase_ray * beta_ray * total_ray // rayleigh color
                    + phase_mie * beta_mie * total_mie // mie
                    + opt_i.x * beta_ambient // and ambient
                    ) * light_intensity + scene_color * opacity; // now make sure the background is rendered correctly
            }

            float3 skylight(float3 sample_pos, float3 surface_normal, float3 light_dir, float3 background_col) {

                // slightly bend the surface normal towards the light direction
                surface_normal = normalize(lerp(surface_normal, light_dir, 0.6));
                
                // and sample the atmosphere
                 return calculate_scattering(
                    _WorldSpaceCameraPos,				// the position of the camera
                    surface_normal, 					// the camera vector (ray direction of this pixel)
                    _ViewRange, 						// max dist, essentially the scene depth
                    float3(0.0, 0.0, 0.0),						// scene color, the color of the current pixel being rendered
                    _LightDir,						// light direction
                    _LightIntensity,						// light intensity, 40 looks nice
                    _Center,						// position of the planet
                    _PlanetRadius,                  // radius of the planet in meters
                    _AtmosphereRadius,                   // radius of the atmosphere in meters
                    _RayBeta,						// Rayleigh scattering coefficient
                    _MieBeta,                       // Mie scattering coefficient
                    _AbsorbtionBeta,                // Absorbtion coefficient
                    _AmbientBeta,					// ambient scattering, turned off for now. This causes the air to glow a bit when no light reaches it
                    _G,                          	// Mie preferred scattering direction
                    _HeightRay,                     // Rayleigh scale height
                    _HeightMie,                     // Mie scale height
                    _HeightAbsorbtion,				// the height at which the most absorption happens
                    _AbsorbtionFalloff,				// how fast the absorption falls off from the absorption height 
                    _Sample, 					// steps in the ray direction 
                    _SampleLight					// steps in the light direction
                );
            }

            bool RayIntersect(float3 origin, float3 rayDirection, float3 center, float3 radius, out float AO, out float BO) {

                float3 L = center - origin;
                float DT = dot(L, rayDirection);
                float R2 = radius * radius;

                float CT2 = dot(L, L) - DT * DT;

                // Intersection point outside the circle
                if (CT2 > R2)
                    return false;
                float AT = sqrt(R2 - CT2);
                float BT = AT;
                AO = DT - AT;
                BO = DT + BT;
                return true;
            }

            float2 ray_sphere_intersect(
                float3 start, // starting position of the ray
                float3 dir, // the direction of the ray
                float radius // and the sphere radius
            ) {
                // ray-sphere intersection that assumes
                // the sphere is centered at the origin.
                // No intersection when result.x > result.y
                float a = dot(dir, dir);
                float b = 2.0 * dot(dir, start);
                float c = dot(start, start) - (radius * radius);
                float d = (b * b) - 4.0 * a * c;
                if (d < 0.0) return float2(1e5, -1e5);
                return float2(
                    (-b - sqrt(d)) / (2.0 * a),
                    (-b + sqrt(d)) / (2.0 * a)
                );
            }



            float3 RayMarching(float3 position, float3 dir) {

                for (int i = 0; i < 64; i++) {
                    if (SphereHit(position, _Center.xyz, _Dist)) {
                        return position;
                    }
                    position += dir * 0.02;
                }
                return float3(0,0,0);
            }


            v2f vert(appdata v)
            {

                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz; // world pos
                return o;
            }



            fixed4 frag(v2f i) : SV_Target
            { 

                float3 viewDir = normalize(i.wPos - _WorldSpaceCameraPos);
                float3 worldPos = i.wPos;

                //float3 depth = RayMarching(worldPos, viewDir);
                //float angle = max(0,dot(depth - _Center, _LightDir));


                //float tA;
                //float tB;
                //bool b = RayIntersect(worldPos, viewDir, _Center, _AtmosphereRadius, tA, tB);

                float3 color = float3(0, 0, 0);
                float3 sceneColor = float3(0, 0, 0);//= skylight(_WorldSpaceCameraPos, viewDir, _LightDir, float3(0.0, 0.0, 0.0));
                color+= calculate_scattering(
                    _WorldSpaceCameraPos,				// the position of the camera
                    viewDir, 					// the camera vector (ray direction of this pixel)
                    _ViewRange, 						// max dist, essentially the scene depth
                    sceneColor,						// scene color, the color of the current pixel being rendered
                    _LightDir,						// light direction
                    _LightIntensity,						// light intensity, 40 looks nice
                    _Center,						// position of the planet
                    _PlanetRadius,                  // radius of the planet in meters
                    _AtmosphereRadius,                   // radius of the atmosphere in meters
                    _RayBeta,						// Rayleigh scattering coefficient
                    _MieBeta,                       // Mie scattering coefficient
                    _AbsorbtionBeta,                // Absorbtion coefficient
                    _AmbientBeta,					// ambient scattering, turned off for now. This causes the air to glow a bit when no light reaches it
                    _G,                          	// Mie preferred scattering direction
                    _HeightRay,                     // Rayleigh scale height
                    _HeightMie,                     // Mie scale height
                    _HeightAbsorbtion,				// the height at which the most absorption happens
                    _AbsorbtionFalloff,				// how fast the absorption falls off from the absorption height 
                    _Sample, 					// steps in the ray direction 
                    _SampleLight					// steps in the light direction
                );
                color = 1.0 - exp(-color);
                if (color.x < 0) {
                    color.x = 0;
                }
                if (color.y < 0) {
                    color.y = 0;
                }
                if (color.z < 0) {
                    color.z = 0;
                }
                
                return  float4(color.x,color.y,color.z,1);

            }
            ENDCG
        }
    }
}
