using Godot;
using System;

[Tool]
public partial class ColorTerrain : Node3D {
  private Texture2D? _heightMap;

  [Export]
  public Texture2D? HeightMap
  {
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

    var surfaceMaterial = (ShaderMaterial) _meshInstance.GetSurfaceOverrideMaterial(0);

    var heightmapTex = ConvertToHeightmap(HeightMap);
    surfaceMaterial.SetShaderParam("u_terrain_accentuation", TerrainAccentuation);
    surfaceMaterial.SetShaderParam("u_heightmap", heightmapTex);
    surfaceMaterial.SetShaderParam("u_tex", HeightMap);

    var image = HeightMap.GetImage();
    SizeMesh(image);
  }

  private static Dictionary<Color, float> _levels = new() {
    // water
    [Color.Color8(158, 217, 246)] = 0f,
    // grass
    [Color.Color8(203,226,163)] = 0f,
    //_sand 
    [Color.Color8(255,250,188)] = 0.2f,
    //_hill 
    [Color.Color8(251,203,114)] = 0.4f,
    //_mountain 
    [Color.Color8(222,163,83)] = 0.6f,
  };

  private Texture2D ConvertToHeightmap(Texture2D heightMap) {
    var texture2D = new ImageTexture();
    
    var newImage = new Image();
    var hmImg = heightMap.GetImage();
    newImage.Create(hmImg.GetWidth(), hmImg.GetHeight(), false, Image.Format.Rgb8);
    var error = newImage.Load(heightMap.ResourcePath);
    if (error != Error.Ok) {
      GD.PrintErr("Cannot load image");
      return texture2D;
    }
    
    var width = hmImg.GetWidth();
    // var width = 10;
    var height = hmImg.GetHeight();
    // var height = 10;
    for (var x = 0; x < width - 1; x++) {
      for (var y = 0; y < height - 1; y++) {
        var color = newImage.GetPixel(x, y);
        // if (x < 10 && y < 10) {
        //   GD.Print(x, " ", y, " ", color.ToHTML(), " =? ", color1.ToHTML());
        // }
        var elevation = _levels.GetValueOrDefault(color, 0f);
        newImage.SetPixel(x, y, new Color(elevation, 0f, 0f));
      }
    }

    texture2D.CreateFromImage(newImage);
    return texture2D;
  }

  private void SizeMesh(Image image) {
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
