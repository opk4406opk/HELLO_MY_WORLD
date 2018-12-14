Shader "Custom/KojeomHalfDiffuse" {
	Properties{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
	#pragma surface surf KojeomHalfLambert
	#pragma target 3.0

	half4 LightingKojeomHalfLambert(SurfaceOutput s, half3 lightDir, half atten) {
		half NdotL = (dot(s.Normal, lightDir) * 0.5f) + 0.5f;
		half4 c;
		c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * NdotL * atten * 2);
		c.a = s.Alpha;
		return c;
	}

	sampler2D _MainTex;
	struct Input {
		float2 uv_MainTex;
	};

		void surf(Input IN, inout SurfaceOutput o) {
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
		}
	ENDCG
	}
		FallBack Off
}
