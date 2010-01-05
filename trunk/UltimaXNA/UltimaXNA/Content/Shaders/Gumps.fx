texture HueTexture : register(t1);
float hueout;

const float HuesPerColumn = 2024;
const float HuesPerRow = 2;

sampler DiffuseSampler : register(s0)  = sampler_state
{
    MipFilter = Point;
    MinFilter = Point;
    MagFilter = Point;
    AddressU  = WRAP;
    AddressV  = WRAP;
};

sampler HueSampler : register(s1)  = sampler_state
{
	Texture   = <HueTexture>;
    MipFilter = LINEAR;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    AddressU  = CLAMP;
    AddressV  = CLAMP;
};

struct VS_OUTPUT
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float4 Color	: COLOR0;
};

float4 PixelShaderFunction(VS_OUTPUT IN) : COLOR0
{
    float4 color = tex2D(DiffuseSampler, IN.TexCoord);
    
    if (!all(IN.Color == 1))
    {
		// This is a hued color
		float HueShade = lerp(0, 255, IN.Color.r) + lerp(0, 255, IN.Color.g) * 256;
		hueout = HueShade;
		float OtherValue = lerp(0, 255, IN.Color.b);
		
		float y = (((HueShade - (HueShade % 2)) / HuesPerRow) / (HuesPerColumn));
		float x = ((color.r + color.g + color.b) / 3.0f / HuesPerRow + (HueShade % 2) * 0.5f).r;
		
		float4 hue = tex2D(HueSampler, float2(x, y));

		// Is it a Partial Hue?
		if (OtherValue != 0) 
		{
			if (color.r == color.g && color.r == color.b && color.a != 0)
			color.rgb = hue.rgb;		
		}
		//Else its a normal Hue
		else 
		{
			color.rgb = hue.rgb;
		}
    }

	return color;
}

technique Gumps
{
    pass Pass0
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
