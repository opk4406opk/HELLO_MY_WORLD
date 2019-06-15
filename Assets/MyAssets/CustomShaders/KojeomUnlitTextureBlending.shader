Shader "Custom/KojeomUnlitTextureBlending"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTexture ("MainTexture (RGB)", 2D) = "white" {}
		_ToTexture("ToTexture (RGB)", 2D) = "white" {}
		_LerpValue("LerpValue", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf NoLighting 

		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			fixed4 c;
			c.rgb = s.Albedo;
			c.a = s.Alpha;
			return c;
		}

        sampler2D _MainTexture;
		sampler2D _ToTexture;

        struct Input
        {
            float2 uv_MainTexture;
			float2 uv_ToTexture;
        };

        fixed4 _Color;
		float _LerpValue;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutput o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = lerp(tex2D(_MainTexture, IN.uv_MainTexture), tex2D(_ToTexture, IN.uv_ToTexture), _LerpValue) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
