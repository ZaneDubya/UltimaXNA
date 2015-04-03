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
	float2 Hue	: TEXCOORD1; //If X = 0, not hued. If X > 0 Hue, if X < 0 partial hue. If Y != 0, target

};

struct PS_INPUT
{
	float4 Position : POSITION0;
	float3 TexCoord : TEXCOORD0;
	float3 Normal	: TEXCOORD1;
	float2 Hue	: TEXCOORD2;
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

float HuesPerColumn = 2024;
float HuesPerRow = 2;

float4 PixelShaderFunction(PS_INPUT IN) : COLOR0
{	
	// Get the initial pixel and discard it if the alpha == 0
	float4 color = tex2D(textureSampler[0], IN.TexCoord);
	if (color.a == 0)
	{
		discard;
	}

	// Darken the color based on the ambient lighting and the normal.
	if (DrawLighting)
	{
		float light_DirectedIntensity = 0.5f + lightIntensity / 2;
		float light_AmbientIntensity = (1.0f - lightIntensity / 10) * ambientLightIntensity;
		float NDotL = saturate(dot(-lightDirection, IN.Normal));
		color.rgb = (light_AmbientIntensity * color.rgb) + (light_DirectedIntensity * NDotL * color.rgb);
	}

	// Hue the color if the hue vector y component is greater than 0.
	if (IN.Hue.y > 0)
	{
		// Hue.Y is a bit flag:
		// 0x01 = completely hued 
		// 0x02 = partially hued
		// 0x04 = 50% transparent.
		// 0x01 & 0x02 should be mutually exclusive.
		float hueIndex = IN.Hue.x; // hue index '0' is true black in Hues.mul
		float hueColumn = (((hueIndex - (hueIndex % 2)) / HuesPerRow) / (HuesPerColumn));
		float gray = ((color.r + color.g + color.b) / 3.0f / HuesPerRow + ((hueIndex % 2) * 0.5f)) * 0.999f;
		float4 huedColor = tex2D(hueTextureSampler, float2(gray, hueColumn));
		huedColor.a = color.a;

		if (IN.Hue.y >= 4) // 50% transparent
		{
			IN.Hue.y %= 4;
			color *= 0.5f;
		}

		if (IN.Hue.y >= 2) // partial hue - map any grayscale pixels to the hue. Colored pixels remain colored.
		{
			if ((color.r == color.g) && (color.r == color.b))
				color = huedColor;
		}
		else if (IN.Hue.y >= 1) // normal hue - map the hue to the grayscale.
		{
			color = huedColor;
		}
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