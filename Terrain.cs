using Godot;
using static Godot.Mathf;

[Tool]
public partial class Terrain : Node3D {
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

  [Export] public float TerrainAccentuation = 31f;

  private void Generate() {
    if (HeightMap is null) {
      return;
    }

    var meshInstance = this.AddNodeIfNotExist(() => new MeshInstance3D(), "Mesh");
    var staticBodyNode = this.AddNodeIfNotExist(() => new StaticBody3D(), "StaticBody");
    var collisionShape =
        staticBodyNode.AddNodeIfNotExist(() => new CollisionShape3D(), "CollisionShape");

    var surfaceMaterial = (ShaderMaterial) meshInstance.GetSurfaceOverrideMaterial(0);
    surfaceMaterial.SetShaderParam("terrain_accentuation", TerrainAccentuation);
    surfaceMaterial.SetShaderParam("u_terrain_heightmap", HeightMap);

    // var mesh = new PlaneMesh();
    // meshInstance.Mesh = mesh;
    var mesh = (PlaneMesh) meshInstance.Mesh;

    var image = HeightMap.GetImage();
    var imageWidth = image.GetWidth();
    var imageHeight = image.GetHeight();

    mesh.SubdivideWidth = imageWidth - 1;
    mesh.SubdivideDepth = imageHeight - 1;
    mesh.Size = new Vector2(1, 1);
    meshInstance.Scale = new Vector3(imageWidth, 1f, imageHeight);

    var heightMapShape = new HeightMapShape3D();
    var mapWidth = imageWidth + 1;
    var mapHeight = imageHeight + 1;
    heightMapShape.MapWidth = mapWidth;
    heightMapShape.MapDepth = mapHeight;
    collisionShape.Shape = heightMapShape;

    if (image.IsCompressed()) {
      GD.PrintErr("Image is compressed");
      return;
    }

    var mapData = new float[mapWidth * mapHeight];

    for (var index = 0; index < mapWidth * mapHeight; index++) {
      var x = FloorToInt((float) index / mapWidth);
      var y = FloorToInt((float) index % mapWidth);
      var height = GetPixelAverage(image, x, y);
      // GD.Print(x, " ", y, " => ", height);
      mapData[index] = height * TerrainAccentuation;
    }

    heightMapShape.MapData = mapData;
  }

  private static float GetPixelAverage(Image image, int x0, int y0) {
    var h00 = GetPixelClamped(image, x0, y0);
    var h10 = GetPixelClamped(image, x0 - 1, y0);
    var h11 = GetPixelClamped(image, x0 - 1, y0 - 1);
    var h01 = GetPixelClamped(image, x0, y0 - 1);

    if (h00 == null && h10 == null && h01 == null && h11 == null) return 0f;

    var sum = (h00?.r ?? 0f) + (h10?.r ?? 0f) + (h01?.r ?? 0f) + (h11?.r ?? 0f);
    var num = (h00 == null ? 0f : 1f) + (h10 == null ? 0f : 1f) + (h01 == null ? 0f : 1f) +
              (h11 == null ? 0f : 1f);
    return sum / num;
  }

  private static Color? GetPixelClamped(Image im, int x, int y) {
    if (x < 0 || x > im.GetWidth() - 1) return null;
    if (y < 0 || y > im.GetHeight() - 1) return null;
    return im.GetPixel(x, y);
  }

  public override void _Ready() {}
}
