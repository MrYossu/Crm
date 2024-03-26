using Crm.Helpers;

namespace Crm.Areas.General.Shared;

public interface WindowParametersInterface {
  string Id { get; set; }
  bool Visible { get; set; }
  string Top { get; set; }
  string Left { get; set; }
  string Width { get; set; }
  string Height { get; set; }
  EventCallback<(Type, string, WindowAttribute, string)> OnWindowResized { get; set; }
  EventCallback<(Type, string)> OnClose { get; set; }
  EventCallback<(Type, string, string)> OnOpenAnotherWindow { get; set; }
}