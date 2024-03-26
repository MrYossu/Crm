
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
if (builder.Environment.IsDevelopment()) {
  builder.Services.AddServerSideBlazor().AddCircuitOptions(options => options.DetailedErrors = true);
}
else {
  builder.Services.AddServerSideBlazor();
}
builder.Services.AddTelerikBlazor();

WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment()) {
  app.UseExceptionHandler("/Error");
  app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToAreaPage("/_Host", "General");

app.Run();