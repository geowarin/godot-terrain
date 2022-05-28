shader_type spatial;

uniform float u_terrain_accentuation = 20.0;

uniform sampler2D u_heightmap: hint_default_black, repeat_disable;
uniform sampler2D u_tex: source_color;

void vertex() {
	// When we scale UVs, we scale around the pivot point of the UVs (0,0)
	// We need to substract 0.5 to move the origin to the center
	vec2 texSize = vec2(textureSize(u_heightmap, 0));
	vec2 scale = texSize / (texSize + 1.);
	UV = (UV - 0.5) * scale + 0.5;
	
	vec4 hm = texture(u_heightmap, UV);

	float h = hm.r * u_terrain_accentuation;
	VERTEX.y = h;
}

void fragment() {
	vec4 albedo_tex = texture(u_tex, UV);
	ALBEDO = albedo_tex.rgb;
}

