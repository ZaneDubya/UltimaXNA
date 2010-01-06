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

float3 DecodeR5G6B5(float2 packed)
{
	// from http://theinstructionlimit.com/?p=33
	// scale up to 8-bit
	packed *= 255.0f;
 
	// round and split the packed bits
	float2 split = round(packed);
	split.x /= 32; // low g in int(3bits), r in frac(5bits).
	split.y /= 8; // b in int(5bits), high g in frac(3bits).
	
	float3 rgb16 = 0.0f.rrr;
	rgb16.r = frac(split.x);
	rgb16.g = frac(split.y) + floor(split.x) / 64;
	rgb16.b = floor(split.y) / 32;
	return rgb16;
}

float4 PixelShaderFunction(VS_OUTPUT IN) : COLOR0
{
    float4 color = tex2D(DiffuseSampler, IN.TexCoord);
    
    if (!all(IN.Color == 1))
    {
		float Flags = round(IN.Color.b * 255.0f);
		
		if (round(Flags) == 2) 
		{
			// This is a real color packed in rgb565
			color.rgb = DecodeR5G6B5(IN.Color.rg);
		}
		else if (round(Flags) == 1)
		{
			// This is a hued color
			float2 hueBytes = round(IN.Color.rg * 63.0f);
			float hueIndex = (hueBytes.x) + floor(hueBytes.y) * 64.0f;
			
			float y = (((hueIndex - (hueIndex % 2)) / HuesPerRow) / HuesPerColumn);
			float x = 0.0f; //((color.r + color.g + color.b) / 3.0f / HuesPerRow + (hueIndex % 2) * 0.5f).r;
			
			float4 hue = tex2D(HueSampler, float2(x, y));

			// Is it a Partial Hue?
			if (Flags && 0x1 == 0x1) 
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
