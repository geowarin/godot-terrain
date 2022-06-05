using Godot;
using System;
using static Godot.Mathf;

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
  private CollisionShape3D? _collisionShape;

  private void Generate() {
    _meshInstance = GetNode<MeshInstance3D>("Mesh");
    _collisionShape = GetNode<CollisionShape3D>("StaticBody/CollisionShape");
    if (HeightMap is null || _meshInstance is null || _collisionShape is null) {
      return;
    }

    var surfaceMaterial = (ShaderMaterial) _meshInstance.GetSurfaceOverrideMaterial(0);

    var heightmapTex = ConvertToHeightmap(HeightMap);
    surfaceMaterial.SetShaderParam("u_terrain_accentuation", TerrainAccentuation);
    surfaceMaterial.SetShaderParam("u_heightmap", heightmapTex);
    surfaceMaterial.SetShaderParam("u_tex", HeightMap);

    _collisionShape.Shape = CreateHeightMapShape(heightmapTex);

    var image = HeightMap.GetImage();
    SizeMesh(image);
  }

  private HeightMapShape3D CreateHeightMapShape(Texture2D heightmapTex) {
    var heightMapShape = new HeightMapShape3D();
    var mapWidth = heightmapTex.GetWidth() + 1;
    var mapHeight = heightmapTex.GetHeight() + 1;
    heightMapShape.MapWidth = mapWidth;
    heightMapShape.MapDepth = mapHeight;

    var mapData = new float[mapWidth * mapHeight];

    for (var index = 0; index < mapWidth * mapHeight; index++) {
      var x = FloorToInt((float) index % mapWidth);
      var y = FloorToInt((float) index / mapWidth);
      var height = ImageUtils.GetPixelAverage(heightmapTex.GetImage(), x, y);
      mapData[index] = height * TerrainAccentuation;
    }
    heightMapShape.MapData = mapData;
    return heightMapShape;
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
    var height = hmImg.GetHeight();
    for (var x = 0; x < width - 1; x++) {
      for (var y = 0; y < height - 1; y++) {
        var color = newImage.GetPixel(x, y);
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
