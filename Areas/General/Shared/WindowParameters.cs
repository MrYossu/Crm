using System.Reflection;
using Crm.Areas.General.Pages;
using Crm.Helpers;

namespace Crm.Areas.General.Shared;

public class WindowParameters : WindowParametersInterface {
  // Ctor used when opening a main window, such as a list. Such windows don't need an Id
  public WindowParameters(Type type) {
    Type = type;
    Title = GetTitle(type);
  }

  private static string GetTitle(Type type) {
    try {
      FieldInfo? titleField = type.GetField("WindowTitle");
      return titleField is null
        ? "unknown"
        : (string)(titleField.GetValue(null) ?? "unknown");
    }
    catch (Exception ex) {
      return $"Ex: {ex.Message}";
    }
  }

  public WindowParameters(WindowManager windowManager, Type type, string id, string title, Action<(Type, string, WindowAttribute, string)> onWindowResized, Func<(Type type, string id), Task> onClose, Func<(Type, string, string), Task> onOpenAnotherWindow) {
    Type = type;
    Id = id;
    Title = !string.IsNullOrWhiteSpace(title) ? title : GetTitle(type);
    OnWindowResized = EventCallback.Factory.Create(windowManager, onWindowResized);
    OnClose = EventCallback.Factory.Create(windowManager, onClose);
    OnOpenAnotherWindow = EventCallback.Factory.Create(windowManager, onOpenAnotherWindow);
  }

  public Type Type { get; }
  public string Id { get; set; } = "";

  public string Title { get; set; }
  public bool Visible { get; set; } = true;
  public EventCallback<(Type, string, WindowAttribute, string)> OnWindowResized { get; set; }
  public EventCallback<(Type, string)> OnClose { get; set; }
  public EventCallback<(Type, string, string)> OnOpenAnotherWindow { get; set; }

  public string Top { get; set; } = "";
  public string Left { get; set; } = "";
  public string Width { get; set; } = "";
  public string Height { get; set; } = "";

  public Dictionary<string, object> GetDictionary() =>
    GetType().GetProperties().Where(property => property.Name != "Title").ToDictionary(property => property.Name, property => property.GetValue(this)!);
}