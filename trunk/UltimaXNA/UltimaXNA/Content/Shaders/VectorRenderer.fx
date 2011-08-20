float4x4 ProjectionMatrix;
float4x4 WorldMatrix;
float2 Viewport;

sampler Texture;

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 TexUV : TexCoord0;
	float4 Hue : COLOR0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 TexUV : TexCoord0;
	float4 Hue : COLOR0;
	float2 Depth : TexCoord1;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	output.Position = mul(mul(input.Position, WorldMatrix), ProjectionMatrix);
	// Half pixel offset for correct texel centering.
	output.Position.x -= 0.5 / Viewport.x;
	output.Position.y += 0.5 / Viewport.y;

	output.Depth = float2((output.Position.z / output.Position.w), 0);
	if (output.Depth.x < .5)
		output.Position.z *= .9;

	output.TexUV = input.TexUV;
	output.Hue = input.Hue;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 color = input.Hue;
    return color;
}

technique Technique1
{
    pass Pass0
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
