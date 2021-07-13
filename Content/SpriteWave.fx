#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
float amount;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{ 	
    float A = 0.01f;
    float w = 10.0f;
    float coorY = sin( w*input.TextureCoordinates.x + amount) * A; 
	coorY = coorY + input.TextureCoordinates.y;

	float4 color = tex2D(SpriteTextureSampler, float2(input.TextureCoordinates.x, coorY)) * input.Color;
	
	return color;
}

technique SpriteDrawing
{
	pass Pass1
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};