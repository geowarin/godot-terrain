using Godot;

public static class ImageUtils {
  public static float GetPixelAverage(Image image, int x0, int y0) {
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
}
