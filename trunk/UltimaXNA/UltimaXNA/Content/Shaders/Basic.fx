float4x4 world : WorldViewProjection;
float3 lightDirection;
bool DrawLighting;

sampler textureSampler;
sampler hueTextureSampler;

float HUETEXTUREHEIGHT = 4096;

struct VS_INPUT
{
	float4 Position : POSITION0;
	float3 Normal	: NORMAL0;
	float2 TexCoord : TEXCOORD0;
	float2 Hue		: TEXCOORD1; //X = Hue, Y = 0 Normal Hue, Y = 1 Partial Hue

};

struct PS_INPUT
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float3 Normal	: TEXCOORD1;
	float2 Hue		: TEXCOORD2;
};

PS_INPUT VertexShader(VS_INPUT IN)
{
	PS_INPUT OUT;
	
    OUT.Position = mul(IN.Position, world);
	OUT.TexCoord = IN.TexCoord; 
	OUT.Normal = IN.Normal;
	OUT.Hue = IN.Hue;
	
    return OUT;
}
/*
float4 PixelShader(PS_INPUT IN) : COLOR0
{		
	float NDotL = saturate(dot(lightDirection, IN.Normal));
	float4 color = tex2D(textureSampler, IN.TexCoord);
	
	if(DrawLighting)
		color *= NDotL;
	
	return color;
}
*/
// New pixelshader created by Jeff - simulates sunrise.

float4 PixelShader(PS_INPUT IN) : COLOR0
{	
	float4 color = tex2D(textureSampler, IN.TexCoord);

	if(DrawLighting)
	{
		float lightIntensity = clamp(dot(lightDirection, float3(0,1,0)), 0, 1);
		float ambientLightIntensity = 0.5f;
		
		float3 lightColor = float3(0.5f + lightIntensity / 2, 0.5f + lightIntensity / 2, 0.5f + lightIntensity / 2);
		float3 ambientColor = float3(1 - lightIntensity / 10, 1 - lightIntensity / 10, 1 - lightIntensity / 10) * ambientLightIntensity;
	
		// float3 lightColor = float3(1, 0.5f + lightIntensity / 2, lightIntensity);
		// float3 lightColor = float3(0.5f + lightIntensity / 2, 0.5f + lightIntensity / 2, 0.5f + lightIntensity / 2);
		// float3 ambientColor = float3(1 - lightIntensity / 10, 1 - lightIntensity / 10, 1 - lightIntensity / 10) * ambientLightIntensity;
		
		float NDotL = saturate(dot(-lightDirection, IN.Normal));

		color.rgb = (ambientColor * color.rgb) + (lightColor * NDotL * color.rgb);
	}

	if (IN.Hue.x > 0 && color.a > 0) //Is it Hued?
	{
		float hueY = (IN.Hue.x / HUETEXTUREHEIGHT);
		float4 gray = (color.r + color.g + color.b) / 3.0;
		float4 hue = tex2D(hueTextureSampler, float2(gray.r, hueY));
		if (IN.Hue.y > 0) //Is it a Partial Hue?
		{
			if (color.r == color.g && color.r == color.b && color.a != 0)
				color = hue;
		}
		else //Else its a normal Hue
		{
			color = hue;
		}
	}

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