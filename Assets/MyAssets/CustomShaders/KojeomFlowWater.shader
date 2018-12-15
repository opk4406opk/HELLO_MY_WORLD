Shader "Custom/KojeomWater" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_ScrollSpeed("ScrollSpeed", Range(0, 10)) = 1.5
		_AlphaValue("AlphaValue", Range(0, 1)) = 0.5
	}
	SubShader {
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		//#pragma surface surf Standard fullforwardshadows
		#pragma surface surf Lambert alpha
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		float _ScrollSpeed;
		float _AlphaValue;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutput o) {
			// 메인텍스처의 UV 좌표값을 계속 변화시킨다.
			//IN.uv_MainTex.y += _Time * _ScrollSpeed;
			if (IN.uv_MainTex.x > 10.0f)
			{
				IN.uv_MainTex.x = 0.0f;
			}
			else
			{
				IN.uv_MainTex.x += _Time * _ScrollSpeed;
			}

			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

			o.Albedo = c.rgb;
			o.Alpha = c.a * _AlphaValue;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
