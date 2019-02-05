Shader "Custom/TextureShader" {
	Properties {
		testTexture("Texture", 2D) = "white"{}
		testScale("Scale", Float) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input {
			float3 worldPos;
			float3 worldNormal;
		};

		const static int maxLayersCount = 8;
		const static float epsilon = 1E-4;

		int layersCount;

		float3 baseColors[maxLayersCount];
		float baseColorsStrength[maxLayersCount];

		float baseBlends[maxLayersCount];

		float baseTextureScales[maxLayersCount];

		float baseStartingHeights[maxLayersCount];
		float minHeight;
		float maxHeight;

		sampler2D testTexture;
		float testScale;

		UNITY_DECLARE_TEX2DARRAY(baseTextures);

		float inverseLerp(float a, float b, float value)
		{
			return saturate((value - a) / (b - a));
		}

		float3 triPlanar(float3 worldPos, float scale, float3 blendAxis, int textureIndex)
		{
			float3 scaledWorldPosition = worldPos / scale;
			
			float3 xProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPosition.y, scaledWorldPosition.z, textureIndex)) * blendAxis.x;
			float3 yProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPosition.x, scaledWorldPosition.z, textureIndex)) * blendAxis.y;
			float3 zProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPosition.x, scaledWorldPosition.y, textureIndex)) * blendAxis.z;

			return xProjection + yProjection + zProjection;
		}

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float heightPercent = inverseLerp(minHeight, maxHeight, IN.worldPos.y);
			// calculate the blend factor for each direction
			float3 blendAxis = abs(IN.worldNormal); // the world-space normal of the fragment. Abs - we don't care, if it's negative or positive
			blendAxis /= (blendAxis.x + blendAxis.y + blendAxis.z); //force it to be within the range of 0 to 1

			for (int i = 0; i < layersCount; i++)
			{
				// how far above the start height the current pixel is. If negative - it's below.
				// interpolates values from 0 to 1. 
				// It starts from when heightPercent is below baseStartingHeights-baseBlends
				// It ends when heightPercent is above baseStartingHeights + baseBlends
				// - epsion prevents division by zero
				float drawStrength = inverseLerp(-baseBlends[i] * 0.5f - epsilon, baseBlends[i] * 0.5f, heightPercent - baseStartingHeights[i]);

				float3 baseColor = baseColors[i] * baseColorsStrength[i];
				float3 textureColor = triPlanar(IN.worldPos, baseTextureScales[i], blendAxis, i) * (1.0f - baseColorsStrength[i]);

				o.Albedo = o.Albedo * (1.0f - drawStrength) + (baseColor + textureColor) * drawStrength;
			}

			// Tri-Planar Texture Mapping 				

			// calculate the blend factor for each direction
			//float3 blendAxis = abs(IN.worldNormal); // the world-space normal of the fragment. Abs - we don't care, if it's negative or positive
			//blendAxis /= (blendAxis.x + blendAxis.y + blendAxis.z); //force it to be within the range of 0 to 1
			//float3 xProjection = tex2D(testTexture, scaledWorldPosition.yz) * blendAxis.x;
			//float3 yProjection = tex2D(testTexture, scaledWorldPosition.xz) * blendAxis.y;
			//float3 zProjection = tex2D(testTexture, scaledWorldPosition.xy) * blendAxis.z;
			//o.Albedo = xProjection + yProjection + zProjection;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
