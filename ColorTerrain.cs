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
    GD.Print("Generate");
    if (HeightMap is null || _meshInstance is null) {
      return;
    }
    
    var surfaceMaterial = (ShaderMaterial) _meshInstance.GetSurfaceOverrideMaterial(0);
    surfaceMaterial.SetShaderParam("terrain_accentuation", TerrainAccentuation);
    surfaceMaterial.SetShaderParam("heightmap", HeightMap);
    
    var mesh = (PlaneMesh) _meshInstance.Mesh;

    var image = HeightMap.GetImage();
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
