// Unlit/WaveTexture 쉐이더 정의
Shader "Unlit/WaveTexture"
{
	Properties
	{
		// 메인 텍스처와 파동 속성 정의
		_MainTex ("Texture", 2D) = "white" {}
		_WaveSpeed ("WaveSpeed", Range(0, 100)) = 10
		_FrequencyX ("X Axis Frequency", Range(0, 100)) = 34
		_AmplitudeX ("X Axis Amplitude", Range(0, 100)) = 0.005
	}

	SubShader
	{
		// 렌더 타입 설정
		Tags { "RenderType"="Opaque" }
		// LOD 설정
		LOD 100

		// 패스
		Pass
		{
			CGPROGRAM
			// 버텍스와 프래그먼트 함수 호출
			#pragma vertex vert
			#pragma fragment frag
			
			// UnityCG.cginc 파일 포함
			#include "UnityCG.cginc"

			// 버텍스 구조체 정의
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			// 출력 버텍스 구조체 정의
			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			// 속성 변수 및 텍스처 선언
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _FrequencyX;
			fixed _AmplitudeX;
			fixed _WaveSpeed;
			
			// 버텍스 쉐이더 함수 정의
			v2f vert (appdata v)
			{
				v2f o;
				// 버텍스 위치 계산
				o.vertex = UnityObjectToClipPos(v.vertex);
				// 텍스처 좌표 계산
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			// 프래그먼트 쉐이더 함수 정의
			fixed4 frag (v2f i) : SV_Target
			{
				// UV 좌표 변수 초기화
				fixed2 uvs = i.uv;
				// X축에 대한 파동 모션 계산
				uvs.x += sin(uvs.y * _FrequencyX + _Time * _WaveSpeed) * _AmplitudeX;
				// 텍스처 좌표를 (0.1, 0.9) 범위로 조정
				uvs.x = (uvs.x * 0.8) + 0.1;
				// 텍스처에서 색상 샘플링
				fixed4 col = tex2D(_MainTex, uvs);
				return col;
			}
			// CGPROGRAM 블록 종료
			ENDCG
		}
	}
}
