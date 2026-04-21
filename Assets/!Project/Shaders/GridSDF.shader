Shader "Custom/GridSDF"
{
    Properties
    {
        _Color ("Color", Color) = (0.5, 0.6, 0.8, 1)
        _Radius ("Corner Radius", Float) = 30
        _Cols ("Columns", Float) = 5
        _Rows ("Rows", Float) = 9
        _EdgeSmooth ("Edge Smoothing", Float) = 1.5
        _RectSize ("Rect Size", Vector) = (1100, 1980, 0, 0)
        _GridTex ("Grid Texture", 2D) = "black" {}

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                float4 color  : COLOR;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float2 uv       : TEXCOORD0;
                float4 worldPos : TEXCOORD1;
                float4 color    : COLOR;
            };

            float4    _Color;
            float     _Radius;
            float     _Cols;
            float     _Rows;
            float     _EdgeSmooth;
            float4    _RectSize;
            float4    _ClipRect;
            sampler2D _GridTex;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex   = UnityObjectToClipPos(v.vertex);
                o.uv       = v.uv;
                o.worldPos = v.vertex;
                o.color    = v.color;
                return o;
            }

            float sdRoundedBoxCorners(float2 p, float2 b, float4 r)
            {
                r.xy = (p.x > 0.0) ? r.yw : r.xz;
                r.x  = (p.y > 0.0) ? r.y  : r.x;
                float2 q = abs(p) - b + r.x;
                return min(max(q.x, q.y), 0.0) + length(max(q, 0.0)) - r.x;
            }

            bool cellActive(int x, int y)
            {
                if (x < 0 || x >= (int)_Cols) return false;
                if (y < 0 || y >= (int)_Rows) return false;
                float2 uv = float2((x + 0.5) / _Cols, 1.0 - (y + 0.5) / _Rows);
                return tex2D(_GridTex, uv).r > 0.5;
            }

            float gridSDF(float2 pixelPos)
            {
                int cols = (int)_Cols;
                int rows = (int)_Rows;

                float cellW = _RectSize.x / _Cols;
                float cellH = _RectSize.y / _Rows;
                float halfW = cellW * 0.5;
                float halfH = cellH * 0.5;
                float r = _Radius;

                float shapeDist = 99999.0;

                // Проход 1: основная форма
                for (int y = 0; y < rows; y++)
                {
                    for (int x = 0; x < cols; x++)
                    {
                        if (!cellActive(x, y)) continue;

                        bool hasLeft  = cellActive(x - 1, y);
                        bool hasRight = cellActive(x + 1, y);
                        bool hasUp    = cellActive(x, y - 1);
                        bool hasDown  = cellActive(x, y + 1);

                        float4 corners = float4(
                            (hasLeft  || hasUp)   ? 0.0 : r,
                            (hasRight || hasUp)   ? 0.0 : r,
                            (hasLeft  || hasDown) ? 0.0 : r,
                            (hasRight || hasDown) ? 0.0 : r
                        );

                        float2 center   = float2((x + 0.5) * cellW, (y + 0.5) * cellH);
                        float2 localPos = pixelPos - center;
                        shapeDist = min(shapeDist, sdRoundedBoxCorners(localPos, float2(halfW, halfH), corners));
                    }
                }

                // Проход 2: вогнутые углы — вычитаем прямо из shapeDist
                for (int sy = 0; sy <= rows; sy++)
                {
                    for (int sx = 0; sx <= cols; sx++)
                    {
                        bool tl = cellActive(sx - 1, sy - 1);
                        bool tr = cellActive(sx,     sy - 1);
                        bool bl = cellActive(sx - 1, sy    );
                        bool br = cellActive(sx,     sy    );

                        int count = (tl?1:0) + (tr?1:0) + (bl?1:0) + (br?1:0);
                        if (count != 3) continue;

                        float2 stitch = float2(sx * cellW, sy * cellH);
                        float2 p = pixelPos - stitch;

                        // Определяем пустую ячейку и её направление
                        float ox = 0.0, oy = 0.0;
                        if (!tl) { ox = -1.0; oy = -1.0; }
                        if (!tr) { ox =  1.0; oy = -1.0; }
                        if (!bl) { ox = -1.0; oy =  1.0; }
                        if (!br) { ox =  1.0; oy =  1.0; }

                        // Пиксель в квадранте пустой ячейки и в пределах r×r
                        bool inQuadrant = (ox * p.x >= 0.0) && (oy * p.y >= 0.0)
                                       && (ox * p.x <= r)   && (oy * p.y <= r);

                        if (inQuadrant)
                        {
                            float circleDist = length(p) - r;
                            // circleDist < 0 = внутри круга = нужно вырезать
                            // делаем shapeDist положительным в этой зоне
                            shapeDist = max(shapeDist, circleDist);

                        }
                    }
                }

                return shapeDist;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 pixelPos = i.uv * _RectSize.xy;

                float dist = gridSDF(pixelPos);

                float alpha = 1.0 - smoothstep(-_EdgeSmooth, _EdgeSmooth, dist);
                alpha *= UnityGet2DClipping(i.worldPos.xy, _ClipRect);

                if (alpha < 0.001) discard;

                fixed4 col = _Color * i.color;
                col.a *= alpha;
                return col;
            }
            ENDCG
        }
    }
}