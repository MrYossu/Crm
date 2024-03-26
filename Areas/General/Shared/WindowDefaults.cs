using Crm.Areas.General.Pages;

namespace Crm.Areas.General.Shared;

public static class WindowDefaults {
  private static readonly Dictionary<Type, (int Width, int Height)> DefaultSizes = new() {
    { typeof(TestComponents), (900, 800) }
  };

  public static string Width(Type type) =>
    DefaultSizes.TryGetValue(type, out (int Width, int Height) value)
      ? $"{value.Width}px"
      : "";

  public static string Height(Type type) =>
    DefaultSizes.TryGetValue(type, out (int Width, int Height) value)
      ? $"{value.Height}px"
      : "";
}