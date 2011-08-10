float4x4 ProjectionMatrix;
float4x4 WorldMatrix;
float2 Viewport;

float3 lightDirection;
float lightIntensity;
float ambientLightIntensity;
bool DrawLighting;

sampler textureSampler[15];
sampler hueTextureSampler;

struct VS_INPUT
{
	float4 Position : POSITION0;
	float3 Normal	: NORMAL0;
	float3 TexCoord : TEXCOORD0;
	float2 Hue		: TEXCOORD1; //If X = 0, not hued. If X > 0 Hue, if X < 0 partial hue. If Y != 0, target

};

struct PS_INPUT
{
	float4 Position : POSITION0;
	float3 TexCoord : TEXCOORD0;
	float3 Normal	: TEXCOORD1;
	float2 Hue		: TEXCOORD2;
};

PS_INPUT VertexShaderFunction(VS_INPUT IN)
{
	PS_INPUT OUT;
	
	OUT.Position = mul(mul(IN.Position, WorldMatrix), ProjectionMatrix);
	
	// Half pixel offset for correct texel centering.
	OUT.Position.x -= 0.5 / Viewport.x;
	OUT.Position.y += 0.5 / Viewport.y;

	OUT.TexCoord = IN.TexCoord; 
	OUT.Normal = IN.Normal;
	OUT.Hue = IN.Hue;
	
    return OUT;
}

// New pixelshader created by Jeff - simulates sunrise.
float HuesPerColumn = 2024;
float HuesPerRow = 2;
float4 PixelShaderFunction(PS_INPUT IN) : COLOR0
{	
	// get the initial color
	float4 color = tex2D(textureSampler[0], IN.TexCoord);
	
	if (IN.TexCoord.x < 0.0f || IN.TexCoord.y < 0.0f ||
		IN.TexCoord.x > 1.0f || IN.TexCoord.y > 1.0f)
		color.a = 0;
	
	if (color.a == 0)
	 	discard;

	// do lighting
	if (DrawLighting)
	{
		float light_DirectedIntensity = 0.5f + lightIntensity / 2;
		float light_AmbientIntensity = (1.0f - lightIntensity / 10) * ambientLightIntensity;
		float NDotL = saturate(dot(-lightDirection, IN.Normal));
		color.rgb = (light_AmbientIntensity * color.rgb) + (light_DirectedIntensity * NDotL * color.rgb);
	}
	
	// is the texture hued or partially transparent?
	// Hue effects are bit flags:
	// 1 = hued 
	// 2 = partially hued
	// 4 = partially transparent.
	// 1 & 2 are mutually exclusive. default to 1 if both exist.
	float hueY = (((IN.Hue.x - (IN.Hue.x % 2)) / HuesPerRow) / (HuesPerColumn));
	float gray = ((color.r + color.g + color.b) / 3.0f / HuesPerRow + ((IN.Hue.x % 2) * 0.5f)) * 0.999f;
	float4 huedColor = tex2D(hueTextureSampler, float2(gray, hueY));
	huedColor.a = color.a;

	if (IN.Hue.y >= 4) // 50% transparent
	{
		IN.Hue.y -= 4;
		color *= 0.5f;
	}

	if (IN.Hue.y == 2) // partial hue - map any grayscale pixels to the hue. Colored pixels remain colored.
	{
		if ((color.r == color.g) && (color.r == color.b))
			color = huedColor;
	}
	else if (IN.Hue.y >= 1) // normal hue - map the hue to the grayscale.
	{
		color = huedColor;
	}

	return color;
}


technique StandardEffect
{
	pass p0
	{
		VertexShader = compile vs_2_0 VertexShaderFunction();
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}