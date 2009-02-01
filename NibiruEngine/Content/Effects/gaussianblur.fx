// Nibiru Engine
// GaussianBlur is step 2 of 3 in the process

sampler TextureSampler : register(s0);

#define SAMPLE_COUNT 15

float2 SampleOffsets[SAMPLE_COUNT];
float SampleWeights[SAMPLE_COUNT];

float4 PixelShader(float2 coordinates : TEXCOORD0) : COLOR0
{
    float4 color = 0;
    
    // Combine a number of weighted image filter taps.
    for (int i = 0; i < SAMPLE_COUNT; i++)
    {
        color += tex2D(TextureSampler, coordinates + SampleOffsets[i]) * SampleWeights[i];
    }
    
    return color;
}

technique GaussianBlur
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShader();
    }
}
