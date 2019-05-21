Shader "Unlit/Screen_PostDistort"
{
    Properties
    {
        [HideInInspector]_MainTex ("Texture", 2D) = "white" {}
		
		_DissoveImage("DissoveImage", 2D) = "white" {}
		_DissoveIntensity("DissoveIntensity", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"
			
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _DissoveImage;
            float4 _DissoveImage_ST;
			float _DissoveIntensity;
			
            v2f vert(appdata_base v) 
			{
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
                return o;
            }

            fixed4 frag (v2f_img i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv + 2*(_DissoveIntensity)*tex2D(_DissoveImage,i.uv) - 1*(_DissoveIntensity));
                return col;
            }
            ENDCG
        }
    }
}