shader_type spatial;

uniform sampler2D heightmap: hint_default_black, repeat_disable;
uniform float terrain_accentuation = 20.0;

uniform sampler2D water: source_color;

void vertex() {
	// When we scale UVs, we scale around the pivot point of the UVs (0,0)
	// We need to substract 0.5 to move the origin to the center
	vec2 texSize = vec2(textureSize(heightmap, 0));
	vec2 scale = texSize / (texSize + 1.);
	UV = (UV - 0.5) * scale + 0.5;
	
	vec4 tex = texture(heightmap, UV);

	float h = tex.r * terrain_accentuation;
	VERTEX.y = h;
}

void fragment() {
	vec4 albedo_tex = texture(heightmap, UV);
	ALBEDO = albedo_tex.rgb;
}

