Shader "Custom/Barrier" {

	Properties {

		_MainTex ("Barrier", 2D) = "white" {}
		[NoScaleOffset] _AlphaTex ("Alpha (GS)", 2D) = "white" {}

		_GlobalAlpha ("Global alpha", Range (0, 1)) = 1
	}

	SubShader {
		
		CGPROGRAM
		#pragma surface surf Lambert alpha:fade

		sampler2D _MainTex;
		sampler2D _AlphaTex;

		float _GlobalAlpha;

		struct Input {

			float2 uv_MainTex;
			float2 uv_AlphaTex;
		};

 		void surf (Input IN, inout SurfaceOutput o) {

			float mt = tex2D(_Maintex, IN.uv_MainTex);
			float at = tex2D(_Alphatex, IN.uv_AlphaTex);

			o.Albedo = 0;
			o.Emission = mt.rgb;
			o.Alpha = mt.a * at.rgb * _GlobalAlpha;
		}
		ENDCG
	}
//	FallBack "Diffuse"
}
