shader_type spatial;

uniform sampler2D texture_albedo : hint_albedo,filter_linear_mipmap,repeat_enable;

uniform sampler2D u_terrain_heightmap: hint_albedo, filter_linear_mipmap, repeat_enable;
uniform float terrain_accentuation = 50.0;

void vertex() {
	float h = texture(u_terrain_heightmap, UV).r * terrain_accentuation;
	VERTEX.y = h;
	UV = UV * 5.0;
}

void fragment() {
	// Place fragment code here.
	vec4 albedo_tex = texture(texture_albedo, UV);
	ALBEDO = albedo_tex.rgb;
}
