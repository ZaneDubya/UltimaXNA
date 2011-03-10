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

	if (color.a == 0)
		discard;

	// do lighting
	if (DrawLighting)
	{
		float3 lightColor = float3(0.5f + lightIntensity / 2, 0.5f + lightIntensity / 2, 0.5f + lightIntensity / 2);
		float3 ambientColor = float3(1 - lightIntensity / 10, 1 - lightIntensity / 10, 1 - lightIntensity / 10) * ambientLightIntensity;
		float NDotL = saturate(dot(-lightDirection, IN.Normal));
		color.rgb = (ambientColor * color.rgb) + (lightColor * NDotL * color.rgb);
	}
	
	if (IN.Hue.y != 0) //Is it Hued?
	{
		float hueX = abs(IN.Hue.x);
		float hueY = (((hueX - (hueX % 2)) / HuesPerRow) / (HuesPerColumn));
		float gray = ((color.r + color.g + color.b) / 3.0f / HuesPerRow + ((hueX % 2) * 0.5f)) * 0.999f;
		
		float4 huedColor = tex2D(hueTextureSampler, float2(gray, hueY));
		huedColor.a = color.a;
		if (IN.Hue.y == 1) //Else its a normal Hue
		{
			color = huedColor;
		}
		else if (IN.Hue.y == 2) //Is it a Partial Hue?
		{
			if ((color.r == color.g) && (color.r == color.b))
				color = huedColor;
		}
		else if (IN.Hue.y == 3) // partially transparent?
		{
			color = huedColor;
			color.a *= 0.5f;
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