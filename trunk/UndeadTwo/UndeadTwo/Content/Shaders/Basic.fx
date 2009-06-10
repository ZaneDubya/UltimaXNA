float4x4 world : WorldViewProjection;
float3 lightDirection;
bool DrawLighting;

sampler textureSampler;

struct VS_INPUT
{
	float4 Position : POSITION;
	float3 Normal	: NORMAL;
	float2 TexCoord : TEXCOORD;
};

struct PS_INPUT
{
	float4 Position : POSITION;
	float2 TexCoord : TEXCOORD0;
	float3 Normal	: TEXCOORD1;
};

PS_INPUT VertexShader(VS_INPUT IN)
{
	PS_INPUT OUT;
	
    OUT.Position = mul(IN.Position, world);
	OUT.TexCoord = IN.TexCoord; 
	OUT.Normal = IN.Normal;
	
    return OUT;
}

float4 PixelShader(PS_INPUT IN) : COLOR0
{		
	float NDotL = saturate(dot(lightDirection, IN.Normal));
	float4 color = tex2D(textureSampler, IN.TexCoord);
	
	if(DrawLighting)
		color *= NDotL;
	
	return color;
}

technique StandardEffect
{
	pass p0
	{
		VertexShader = compile vs_2_0 VertexShader();
		PixelShader = compile ps_2_0 PixelShader();
	}
}

/*
float4x4 world : WorldViewProjection;
float3 lightDirection : Direction;
bool DrawLighting;

sampler textureSampler;

struct VS_INPUT
{
	float4 Position : POSITION;
	float3 Normal : NORMAL;
	float2 TexCoord : TEXCOORD;
};

struct PS_INPUT
{
	float4 Position : POSITION;
	float2 TexCoord : TEXCOORD0;
	float4 Normal : TEXCOORD1;
};

PS_INPUT VertexShader(VS_INPUT Input)
{
	PS_INPUT Output;
    Output.Position = mul(Input.Position, world);
	Output.TexCoord = Input.TexCoord; 
	Output.Normal = dot(normalize(lightDirection), Input.Normal);
    return Output;
}

float4 PixelShader(PS_INPUT Input) : COLOR0
{	
	float4 Output;
	if (DrawLighting)
	{
		Output = Input.Normal * tex2D(textureSampler, Input.TexCoord);
	}
	else
	{
		Output = tex2D(textureSampler, Input.TexCoord);
	}
	return Output;
}

technique StandardEffect
{
	pass p0
	{
		VertexShader = compile vs_2_0 VertexShader();
		PixelShader = compile ps_2_0 PixelShader();
	}
}
*/


























float4 PS_GreyScale(PS_INPUT Input) : COLOR0
{	
	float4 iPixel = tex2D(textureSampler, Input.TexCoord);
	iPixel.rgb = dot(iPixel.rgb, float3(0.3, 0.59, 0.11)); //compose correct luminance value
	return iPixel;
}

float4 PS_Sharpen(PS_INPUT Input) : COLOR0
{	
	float4 iPixel = tex2D(textureSampler, Input.TexCoord);
	
	iPixel -= tex2D(textureSampler, Input.TexCoord + 0.01f) * 10.0f; 
    iPixel += tex2D(textureSampler, Input.TexCoord - 0.01f) * 10.0f;
	return iPixel;
}

float4 PS_Negative(PS_INPUT Input) : COLOR0
{	
	float4 iPixel = 1 - tex2D(textureSampler, Input.TexCoord);
	return iPixel;
}