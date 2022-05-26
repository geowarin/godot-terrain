using Godot;

[Tool]
public partial class Terrain : MeshInstance3D {
  [Export] public Texture2D? HeightMap;
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
  
  [Export]
  public float TerrainAccentuation = 31f;

  private const int _subdivisions = 10;
  private const int _cellSize = 10;

  private void Generate() {
    if (HeightMap is null) {
      return;
    }
    
    var staticBodyNode = this.AddNodeIfNotExist(() => new StaticBody3D(), "StaticBody");
    var collisionShape =
        staticBodyNode.AddNodeIfNotExist(() => new CollisionShape3D(), "CollisionShape");
    
    var surfaceMaterial = (ShaderMaterial) GetSurfaceOverrideMaterial(0);
    surfaceMaterial.SetShaderParam("terrain_accentuation", TerrainAccentuation);
    surfaceMaterial.SetShaderParam("u_terrain_heightmap", HeightMap);
    
    var mesh = (PlaneMesh) Mesh;
    
    mesh.SubdivideWidth = _subdivisions;
    mesh.SubdivideDepth = _subdivisions;
    mesh.Size = new Vector2(_cellSize * _subdivisions, _cellSize * _subdivisions);
    
    collisionShape.Scale = new Vector3(_cellSize, 1f, _cellSize);

    var image = HeightMap.GetImage();
    var mapWidth = image.GetWidth() + 1;
    var mapHeight = image.GetHeight() + 1;

    var heightMapShape = new HeightMapShape3D();
    heightMapShape.MapWidth = mapWidth;
    heightMapShape.MapDepth = mapHeight;
    collisionShape.Shape = heightMapShape;
    // heightMapShape.MapData = new float[mapWidth * mapHeight];

    if (image.IsCompressed()) {
      GD.PrintErr("Image is compressed");
      return;
    }

    var mapData = new float[mapWidth * mapHeight];
    for (var index = 0; index < mapWidth * mapHeight; index++) {
      var x = Mathf.FloorToInt((float) index / mapWidth);
      var y = Mathf.FloorToInt((float) index % mapWidth);
      var height = GetPixelAverage(image, x, y);
      mapData[index] = height * TerrainAccentuation;
    }

    heightMapShape.MapData = mapData;
  }

  private float GetPixelAverage(Image image, int vertX, int vertY) {
    //image.GetPixel().r
    return 0.0f;
  }

  public override void _Ready() {}
}
