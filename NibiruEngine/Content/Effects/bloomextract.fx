// Nibiru Engine
// BloomExtract is step 1 of 3 in the process

sampler TextureSampler : register(s0);

float BloomThreshold;

float4 PixelShader(float2 coordinates : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(TextureSampler, coordinates);
    return saturate((color - BloomThreshold) / (1 - BloomThreshold));
}

technique BloomExtract
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShader();
    }
}
