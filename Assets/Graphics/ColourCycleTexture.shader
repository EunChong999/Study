// Unlit/ColourCycleTexture 쉐이더 정의
Shader "Unlit/ColourCycleTexture"
{
	Properties
	{
		// 메인 텍스처와 색상 속성 정의
		_MainTex("Color (RGB) Alpha (A)", 2D) = "white" {}
		_WaveSpeed ("WaveSpeed", Range(-1000, 1000)) = 20
		_Frequency ("Frequency", Range(0, 100)) = 10
		_Amplitude ("Amplitude", Range(0, 3)) = 0.02
		_RedScale("Red Scale", Range(0, 3)) = 1
		_GreenScale("Green Scale", Range(0, 3)) = 1
		_BlueScale("Blue Scale", Range(0, 3)) = 1
	}

	SubShader
	{
		// 렌더 큐와 렌더 타입 설정
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		// 알파 블렌딩 및 LOD 설정
		Blend SrcAlpha OneMinusSrcAlpha
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
			fixed _Frequency;
			fixed _WaveSpeed;
			fixed _Amplitude;
			fixed _RedScale;
			fixed _GreenScale;
			fixed _BlueScale;

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
				// UV 좌표 및 텍스처 색상 변수 선언
				fixed2 uvs = i.uv;
				fixed4 col = tex2D(_MainTex, uvs);
				// 파동 모션 계산
				fixed d = sin(uvs.y * _Frequency + _Time * _WaveSpeed) * _Amplitude;
				// 색상에 파동 모션 및 스케일 적용
				col.r += d * _RedScale;
				col.g += d * _GreenScale;
				col.b += d * _BlueScale;
				return col;
			}
			// CGPROGRAM 블록 종료
			ENDCG
		}
	}
}
