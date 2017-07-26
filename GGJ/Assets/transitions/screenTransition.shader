Shader "Hidden/screenTransition"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_TransitionTex("Texture",2D) = "white" {}
		_Cutoff("Cutoff", Range(0,1)) = 0
		_Color("Color", Color) = (1,1,1,1)
		[MaterialToggle] _Distort("Distort", Float) = 0
		_fade("fade", Range(0,1)) = 0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			float4 _MainTex_TexelSize;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.uv1 = v.uv;

				#if UNITY_UV_STARTS_AT_TOP
					if(_MainTex_TexelSize.y < 0)
					o.uv1.y = 1 - o.uv1.y; 
				#endif
				
				return o;
			}
			
			sampler2D _MainTex;
			
			sampler2D _TransitionTex;
			float _Cutoff;
			int _Distort;
			fixed4 _Color;
			float _fade;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 transit = tex2D(_TransitionTex, i.uv);
				fixed2 direction = float2(0,0);

				if(_Distort)
						direction = normalize(float2((transit.r - 0.5) * 2, (transit.g - 0.5) * 2));

				fixed4 col = tex2D(_MainTex, i.uv + _Cutoff * direction);

				if(transit.b < _Cutoff)
					return col = lerp(col, _Color, _fade);

				return col;
			}
			ENDCG
		}
	}
}
