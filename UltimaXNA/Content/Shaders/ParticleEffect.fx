uniform extern float4x4 worldViewProjection : WORLDVIEWPROJECTION;

float currentTime;

// emitter parameters

float lifetime;

bool moveWithEmitter;
float3 emitterLocationDiff;

bool useMovement;
bool changeMovement;
bool changeVelocity;

bool useColor;
bool changeColor;
bool useAlpha;
bool changeAlpha;

bool useTexture;
uniform extern texture effectTexture;

bool changeSize;


sampler textureSampler = sampler_state
{
    Texture = <effectTexture>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

struct VertexShaderInput
{
    float3 Position : POSITION0;
    float  CreationTime : PSIZE0;
    float3 MovementDirectionStart : NORMAL0;
    float3 MovementDirectionEnd : NORMAL1;
    float  VelocityStart : PSIZE1;
    float  VelocityEnd : PSIZE2;
    float4 ColorStart : COLOR0;
    float4 ColorEnd : COLOR1;
    float  SizeStart : PSIZE3;
    float  SizeEnd : PSIZE4;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float  Size : PSIZE0;
};

float4 CalcParticlePosition(float3 position, 
		float3 movementDirectionStart, float3 movementDirectionEnd,
		float velocityStart, float velocityEnd, 
		float normalizedAge) : POSITION0
{
	if (useMovement) {
		if (changeMovement) {
			movementDirectionStart = movementDirectionStart * normalizedAge + 
					(movementDirectionEnd - movementDirectionStart) * normalizedAge * normalizedAge / 2;
			movementDirectionStart = normalize(movementDirectionStart);
		}
	
		if (!changeVelocity) {
			velocityEnd = velocityStart;
		}
	
		// Our particles have constant acceleration, so given a starting velocity
		// S and ending velocity E, at time T their velocity should be S + (E-S)*T.
		// The particle position is the sum of this velocity over the range 0 to T.
		// To compute the position directly, we must integrate the velocity
		// equation. Integrating S + (E-S)*T for T produces S*T + (E-S)*T*T/2.
		velocityStart = velocityStart * normalizedAge + 
					(velocityEnd - velocityStart) * normalizedAge * normalizedAge / 2;
		
		position += movementDirectionStart * velocityStart * lifetime;
	}
	
	if (moveWithEmitter) {
		position += emitterLocationDiff;
	}
	
	return mul(float4(position, 1), worldViewProjection);
}

float4 CalcParticleColor(float4 colorStart, float4 colorEnd, float normalizedAge) : COLOR0
{
	float4 ret = float4(1, 1, 1, 1);

	if (useColor) {
		ret.rgb = colorStart;
		if (changeColor) {
			ret.rgb = lerp(colorStart.rgb, colorEnd.rgb, normalizedAge);
		}
	}
	
	if (useAlpha) {
		if (changeAlpha) {
			ret.a = lerp(colorStart.a, colorEnd.a, normalizedAge);
		} else {
			ret.a = colorStart.a;
		}
	} else {
		ret.a = 1;
	}
	
	return ret;
}

float CalcParticleSize(float sizeStart, float sizeEnd, float normalizedAge) : PSIZE0
{
	if (changeSize) {
		return lerp(sizeStart, sizeEnd, normalizedAge);
	} else {
		return sizeStart;
	}
}

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;
    
    // particle age and age randomness
    float age = currentTime - input.CreationTime;
    
    float normalizedAge = (age / lifetime);
    
    output.Position = CalcParticlePosition(input.Position,
		input.MovementDirectionStart, input.MovementDirectionEnd,
		input.VelocityStart, input.VelocityEnd, 
		normalizedAge);
	output.Color = CalcParticleColor(input.ColorStart, input.ColorEnd, normalizedAge);
    output.Size = CalcParticleSize(input.SizeStart, input.SizeEnd, normalizedAge);

    return output;
}	


struct PixelShaderInput 
{
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 PixelShaderFunction(PixelShaderInput input) : COLOR0
{
	if (useTexture) {
		return input.Color * tex2D(textureSampler, input.TextureCoordinates).rgba;
	} else {
		return input.Color;
	}
}

technique T0
{
    pass P0
    {
        VertexShader = compile vs_1_1 VertexShaderFunction();
        PixelShader = compile ps_1_1 PixelShaderFunction();
    }
}
