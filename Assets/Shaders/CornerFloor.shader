Shader "Custom/CornerFloor"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        // Add properties for the corner tiles
        _CornerTile1 ("Corner Tile 1", 2D) = "white" {}
        _CornerTile2 ("Corner Tile 2", 2D) = "white" {}
        _CornerTile3 ("Corner Tile 3", 2D) = "white" {}
        _CornerTile4 ("Corner Tile 4", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _CornerTile1;
        sampler2D _CornerTile2;
        sampler2D _CornerTile3;
        sampler2D _CornerTile4;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Forward declaration of the CheckCornerTile function
        bool CheckCornerTile(float2 uv);

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Determine the corner tile based on the neighboring tiles
            half4 cornerTile = tex2D(_CornerTile1, IN.uv_MainTex);

            if (CheckCornerTile(IN.uv_MainTex + float2(-1, -1)))
                cornerTile = tex2D(_CornerTile2, IN.uv_MainTex);
            else if (CheckCornerTile(IN.uv_MainTex + float2(1, -1)))
                cornerTile = tex2D(_CornerTile3, IN.uv_MainTex);
            else if (CheckCornerTile(IN.uv_MainTex + float2(1, 1)))
                cornerTile = tex2D(_CornerTile4, IN.uv_MainTex);

            // Blend the corner tile with the main texture
            half4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
            half4 blendedColor = lerp(mainTex, cornerTile, cornerTile.a);

            // Apply the color and properties to the surface output
            half4 finalColor = blendedColor * _Color;
            o.Albedo = finalColor.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = finalColor.a;
        }

        bool CheckCornerTile(float2 uv)
        {
            half4 cornerTile = tex2D(_MainTex, uv);
            return cornerTile.a > 0.5; // Adjust the threshold as needed
        }
        ENDCG
    }
    FallBack "Diffuse"
}
