using Crm.Areas.General.Shared;
using Crm.Helpers;

namespace Crm.Areas.General.Pages;

public partial class WindowManager {
  #region Inject Props & Fields

  [Inject]
  public IJSRuntime JsRuntime { get; set; } = null!;

  public IJSObjectReference? Module { get; set; }
  public string Title { get; set; } = "CRM";
  public ObservableCollection<WindowParameters> Windows { get; set; } = [];

  [Parameter]
  public string SelectedWindowId { get; set; } = "";

  private bool PinNavMenu { get; }

  private const int WindowBitsType = 0;
  private const int WindowBitsId = 1;
  private const int WindowBitsTop = 2;
  private const int WindowBitsLeft = 3;
  private const int WindowBitsTitle = 4;

  #endregion

  protected override void OnInitialized() =>
    Windows.CollectionChanged += Windows_CollectionChanged;

  private void Windows_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) =>
    Console.WriteLine($"WindowManager.Windows_CollectionChanged - {DateTime.Now.ToLongTimeString()}");

  [JSInvokable("SetWindowTitle")]
  public void SetWindowTitle(string type, string id) {
    SelectedWindowId = $"{type}|{id}";
    WindowParameters wp = Windows.Single(i => i.Type.Name == type && id == i.Id);
    Title = wp.Title;
    StateHasChanged();
  }

  private async Task Open(WindowParameters parameters) {
    Console.WriteLine("Open");
    WindowParameters? wp = Windows.SingleOrDefault(i => i.Type == parameters.Type && parameters.Id == i.Id);
    if (wp is null) {
      Console.WriteLine($"WindowManager.Open - Type: {parameters.Type.Name}, Position: ({parameters.Top}, {parameters.Left}, Size: ({parameters.Height}, {parameters.Width}))");
      parameters.Visible = false;
      Windows.Add(parameters);
      parameters.Visible = true;
    } else {
      if (Module is not null) {
        await Module.InvokeVoidAsync("bringToFront", $".igwindow_{wp.Type.Name}_{wp.Id}");
      }
    }
  }

  private void WindowResized((Type type, string id, WindowAttribute att, string value) data) {
    WindowParameters? wp = Windows.SingleOrDefault(i => i.Type == data.type && data.id == i.Id);
    try {
      if (wp is not null) {
        switch (data.att) {
          case WindowAttribute.Top:
            double topIntVal = double.Parse(data.value.Replace("px", ""));
            if (topIntVal < 57) {
              data.value = "57px";
            }
            wp.Top = data.value;
            break;
          case WindowAttribute.Left:
            double leftIntVal = double.Parse(data.value.Replace("px", ""));
            if (PinNavMenu) {
              if (leftIntVal < 270) {
                data.value = "270px";
              }
            } else {
              if (leftIntVal < 72) {
                data.value = "72px";
              }
            }
            wp.Left = data.value;
            break;
          case WindowAttribute.Width:
            wp.Width = data.value;
            break;
          case WindowAttribute.Height:
            wp.Height = data.value;
            break;
        }
      } else {
        Console.WriteLine(" Ulp, can't find that one!");
      }
    }
    catch (Exception ex) {
      Console.WriteLine($"WindowResized error: {ex.Message}");
    }
  }

  private async Task Close((Type type, string id) data) {
    Windows.Remove(Windows.Single(w => w.Type == data.type && w.Id == data.id));
    string className = await JsRuntime.InvokeAsync<string>("getActive", WindowBase.WindowClass(data.type.Name, data.id));
    if (!string.IsNullOrWhiteSpace(className)) {
      string[] bits = className.Split('_');
      WindowParameters? wp = Windows.SingleOrDefault(w => w.Type.Name == bits[1] && w.Id == bits[2]);
      Title = wp is null ? "CRM" : wp.Title;
      SelectedWindowId = $"{bits[1]}|{bits[2]}";
    } else {
      // This should only happen if they just closed the last window
      Title = "CRM";
    }
  }

  private Task OpenAnotherWindow((Type type, string id, string title) data) =>
    Open(new(this, data.type, data.id, data.title, WindowResized, Close, OpenAnotherWindow));

  private Task OpenAnotherWindow(WindowParameters wp) {
    wp.OnWindowResized = EventCallback.Factory.Create<(Type, string, WindowAttribute, string)>(this, WindowResized);
    wp.OnClose = EventCallback.Factory.Create<(Type, string)>(this, Close);
    wp.OnOpenAnotherWindow = EventCallback.Factory.Create<(Type, string, string)>(this, OpenAnotherWindow);
    return Open(wp);
  }

  private Task OpenTest() {
    WindowParameters wp = new(typeof(TestComponents)) { Top = "200px", Left = "200px", Height = "300px", Width = "300px" };
    return OpenAnotherWindow(wp);
  }
}