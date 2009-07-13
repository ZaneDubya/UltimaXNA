sampler textureSampler;
sampler maskSampler;

struct VS_INPUT
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};

struct PS_INPUT
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};

PS_INPUT VertexShader(VS_INPUT IN)
{
	PS_INPUT OUT;
	
    OUT.Position = IN.Position;
	OUT.TexCoord = IN.TexCoord; 
	
    return OUT;
}

float4 PixelShader(PS_INPUT IN) : COLOR0
{	
	float4 color = lerp(tex2D(textureSampler, IN.TexCoord), 0, tex2D(maskSampler, IN.TexCoord).r);
	return color;
}


technique MaskEffect
{
	pass p0
	{
		VertexShader = compile vs_2_0 VertexShader();
		PixelShader = compile ps_2_0 PixelShader();
	}
}