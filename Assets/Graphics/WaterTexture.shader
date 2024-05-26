// Unlit/WaterTexture 쉐이더 정의
Shader "Unlit/WaterTexture"
{
	Properties
	{
		// 메인 텍스처와 스크롤, 파동 속성 정의
		_MainTex ("Texture", 2D) = "white" {}
		_ScrollX ("X Scroll Speed", Range(-10, 10)) = 0
		_WaveSpeed("WaveSpeed", Range(0, 100)) = 10
		_FrequencyX("X Axis Frequency", Range(0, 100)) = 34
		_AmplitudeX("X Axis Amplitude", Range(0, 100)) = 0.005
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
			fixed _ScrollX;
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
			fixed4 frag(v2f i) : SV_Target
			{
				// 현재 시간과 파동 속도 계산
				fixed time = _Time * _WaveSpeed;
				// UV 좌표 및 오프셋 변수 초기화
				fixed2 uvs = i.uv;
				fixed offset = (_ScrollX * _Time);
				// 텍스처 스크롤링 계산
				uvs.x += offset;
				uvs.x = fmod(uvs.x, 1);
				// X축에 대한 파동 모션 계산
				uvs.x += sin(uvs.y * _FrequencyX + time) * _AmplitudeX;
				// 두 번째 텍스처의 UV 좌표 및 계산
				fixed2 uvs2 = i.uv;
				uvs2.x -= offset;
				uvs2.x = fmod(uvs2.x, 1);
				if (uvs2.x < 0)
					uvs2.x += 1;
				uvs2.y = 1 - i.uv.y;
				uvs2.x += sin(uvs2.y * _FrequencyX + time) * _AmplitudeX;
				// 두 텍스처의 색상 계산 및 평균화
				fixed4 col = tex2D(_MainTex, uvs);
				fixed4 col2 = tex2D(_MainTex, uvs2);
				col = (col + col2) / 2;
				return col;
			}
			// CGPROGRAM 블록 종료
			ENDCG
		}
	}
}
