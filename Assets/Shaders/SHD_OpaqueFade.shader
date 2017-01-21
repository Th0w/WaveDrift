Shader "Custom/SimpleDiffuse" {

	Properties {

		_MainColor ("Main color", Color) = (1,1,1,1)
		_FresnelColor ("Fresnel color", Color) = (1,1,1,1)

		_FresnelPow ("Fresnel power", Float) = 1

		_OverGlow ("OverGlow", Float) = 0
	}

	SubShader {

		CGPROGRAM
		#pragma surface surf Lambert

		float4 _MainColor;
		float4 _FresnelColor;

		float _FresnelPow;

		float _OverGlow;

		struct Input {

			float3 viewDir;
		};

		void surf (Input IN, inout SurfaceOutput o) {

			o.Albedo = 0;

			float fresnel = pow(dot(IN.viewDir, o.Normal), _FresnelPow);
			o.Emission = _MainColor.rgb + _FresnelColor.rgb * fresnel * _OverGlow;
		}
		ENDCG
	}
//	FallBack "Diffuse"
}
