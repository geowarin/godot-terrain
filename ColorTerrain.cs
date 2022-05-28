using Godot;
using System;

[Tool]
public partial class ColorTerrain : Node3D {
  private Texture2D? _heightMap;
  [Export] public Texture2D? HeightMap {
    get => _heightMap;
    set
    {
      _heightMap = value;
      Generate();
    }
  }
  private bool _reload;

  [Export]
  public bool Reload
  {
    get => _reload;
    set
    {
      _reload = value;
      Generate();
    }
  }

  [Export] public float TerrainAccentuation = 31f;
  private MeshInstance3D? _meshInstance;

  private void Generate() {
    _meshInstance = GetNode<MeshInstance3D>("Mesh");
    if (HeightMap is null || _meshInstance is null) {
      return;
    }
    GD.Print("Generate");
    
    var surfaceMaterial = (ShaderMaterial) _meshInstance.GetSurfaceOverrideMaterial(0);

    Texture2D heightmapTex = ConvertToHeightmap(HeightMap);
    surfaceMaterial.SetShaderParam("u_terrain_accentuation", TerrainAccentuation);
    surfaceMaterial.SetShaderParam("u_heightmap", heightmapTex);
    surfaceMaterial.SetShaderParam("u_tex", HeightMap);
    
    var image = HeightMap.GetImage();
    SizeMesh(image);
  }

  private Texture2D ConvertToHeightmap(Texture2D heightMap) {
    var texture2D = new ImageTexture();
    texture2D.CreateFromImage(heightMap.GetImage());
    return texture2D;
  }

  private void SizeMesh(Image image)
  {
    var mesh = (PlaneMesh) _meshInstance.Mesh;
    var imageWidth = image.GetWidth();
    var imageHeight = image.GetHeight();

    mesh.SubdivideWidth = imageWidth - 1;
    mesh.SubdivideDepth = imageHeight - 1;
    mesh.Size = new Vector2(1, 1);
    _meshInstance.Scale = new Vector3(imageWidth, 1f, imageHeight);
  }

  public override void _Ready() {
    _meshInstance = GetNode<MeshInstance3D>("Mesh");
  }
}
