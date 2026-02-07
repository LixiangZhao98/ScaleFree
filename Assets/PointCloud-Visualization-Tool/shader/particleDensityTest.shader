Shader "Custom/PointDensityColormap"
{
    Properties
    {
        _PointSize("Point Size", Float) = 10
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // 点大小支持（仅用于GL/DirectX，部分平台不支持）
            float _PointSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 uv1 : TEXCOORD1; // 用 SetUVs(1, ...) 传归一化密度
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float density : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.density = v.uv1.x; // 只取归一化密度分量
                #if defined(SHADER_API_D3D11) || defined(SHADER_API_GLCORE)
                o.pos.w = o.pos.w + 0.00001; // trick for correct point rendering
                #endif
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float density = saturate(i.density);
                float3 c1 = float3(0,0,1);   // 蓝
                float3 c2 = float3(1,1,0);   // 黄
                float3 c3 = float3(1,0,0);   // 红

                float3 color;
                if (density < 0.5)
                    color = lerp(c1, c2, density * 2.0);
                else
                    color = lerp(c2, c3, (density-0.5)*2.0);

                return float4(color, 1.0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
