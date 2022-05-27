shader_type spatial;

uniform sampler2D texture_albedo : source_color,filter_linear_mipmap,repeat_enable;

uniform sampler2D u_terrain_heightmap: hint_default_black, repeat_disable;
uniform float terrain_accentuation = 50.0;

void vertex() {
	// When we scale UVs, we scale around the pivot point of the UVs (0,0)
	// We need to substract 0.5 to move the origin to the center
	vec2 texSize = vec2(textureSize(u_terrain_heightmap, 0));
	vec2 scale = texSize / (texSize + 1.);
	UV = (UV - 0.5) * scale + 0.5;

	float h = texture(u_terrain_heightmap, UV).r * terrain_accentuation;
	VERTEX.y = h;
}

void fragment() {
	// Place fragment code here.
	vec4 albedo_tex = texture(texture_albedo, UV);
	ALBEDO = albedo_tex.rgb;
}
