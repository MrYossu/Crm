using Crm.Helpers;

namespace Crm.Areas.General.Shared;

public class WindowBase : CrmCommonComponentBase, WindowParametersInterface {

  #region Window parameters

  [Parameter]
  // I know this looks like it isn't used, but it is used implicitly, and if you remove it, bad things will happen, so don't!
  public Type Type { get; set; } = null!;

  [Parameter]
  public string Id { get; set; } = "";

  [Parameter]
  public bool Visible { get; set; } = true;

  [Parameter]
  public string Top { get; set; } = "";

  [Parameter]
  public string Left { get; set; } = "";

  [Parameter]
  public string Width { get; set; } = "";

  [Parameter]
  public string Height { get; set; } = "";

  #endregion

  #region OnParametersSet

  protected TelerikWindow? WindowRef { get; set; }

  protected override void OnParametersSet() {
    Console.WriteLine($"WindowBase.OnParametersSet - Type: {Type.Name}");
    if (WindowRef is not null) {
      #pragma warning disable BL0005
      WindowRef.Visible = Visible;
      WindowRef.Top = Top;
      WindowRef.Left = Left;
      WindowRef.Width = Width;
      WindowRef.Height = Height;
      WindowRef.LeftChanged = new EventCallbackFactory().Create<string>(this, s => WindowResized(WindowAttribute.Left, $"{s}"));
      WindowRef.HeightChanged = new EventCallbackFactory().Create<string>(this, s => WindowResized(WindowAttribute.Height, $"{s}"));
      WindowRef.TopChanged = new EventCallbackFactory().Create<string>(this, s => WindowResized(WindowAttribute.Top, $"{s}"));
      WindowRef.WidthChanged = new EventCallbackFactory().Create<string>(this, s => WindowResized(WindowAttribute.Width, $"{s}"));
      string winClass = WindowClass(Type.Name, Id);
      if ((WindowRef.Class ?? "").IndexOf(winClass, StringComparison.Ordinal) == -1) {
        WindowRef.Class += winClass;
      }
      #pragma warning restore BL0005
    }
  }

  public static string WindowClass(string type, string id) =>
    $"igwindow_{type}_{id}";

  [Parameter]
  // Raised whenever a window is moved or resized. Sends the window type, Id, parameter that changed (left, top, width or height) and the new value
  public EventCallback<(Type, string, WindowAttribute, string)> OnWindowResized { get; set; }

  public async Task WindowResized(WindowAttribute att, string value) =>
    await OnWindowResized.InvokeAsync((GetType(), Id, att, value));

  #endregion

  #region OnClose

  [Parameter]
  public EventCallback<(Type, string)> OnClose { get; set; }

  public Task Close() =>
    OnClose.InvokeAsync((GetType(), Id));

  #endregion

  #region OnOpenAnotherWindow

  [Parameter]
  public EventCallback<(Type, string, string)> OnOpenAnotherWindow { get; set; }

  public Task OpenAnotherWindow(Type type, string id = "", string title = "") =>
    OnOpenAnotherWindow.InvokeAsync((type, id, title));

  #endregion
}