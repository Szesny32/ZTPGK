Shader "Custom/Interpolation"
{
    Properties
    {
        _ShapeTex ("Shape", 2D) = "grey" {}
        _ColorA ("Color A", Color) = (1,0,0,1)
        _ColorB ("Color B", Color) = (0,0,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
 
            #include "UnityCG.cginc"
 
            struct v2f
            {
                float4 pos : SV_POSITION;
                float weight : TEXCOORD0;
            };
 
            sampler2D _ShapeTex;
            float4 _ShapeTex_ST;
 
            float4 _ColorA, _ColorB;
 
            v2f vert (appdata_full v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
 
                // getting the weights from a random texture because I didn't want to have to import in a vertex painted mesh
                o.weight = tex2Dlod(_ShapeTex, float4(TRANSFORM_TEX(v.texcoord.xy, _ShapeTex), 0, 0)).r;
 
                // uncomment this to test out what it looks like to use binary values of 0 or 1 for the weight
                // o.weight = round(o.weight);
 
                // example of weights stored in the vertex color, how you'd probably _actually_ want to do it
                // o.weight = v.color.r;
 
                return o;
            }
 
            fixed4 frag (v2f i) : SV_Target
            {
                // preview the weights
                // return i.weight;
 
                // anti-aliased sharp weight
                float weight = saturate((i.weight - 0.5) / max(0.00001, fwidth(i.weight)));
 
                // lerp between the two colors
                return lerp(_ColorA, _ColorB, weight);
            }
            ENDCG
        }
    }
}