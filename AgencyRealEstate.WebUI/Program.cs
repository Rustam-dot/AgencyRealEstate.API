using AgencyRealEstate.WebUI.Auth;
using AgencyRealEstate.WebUI.Components;
using AgencyRealEstate.WebUI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// --- MudBlazor ---
builder.Services.AddMudServices();

// --- HTTP-контекст для чтения куки ---
builder.Services.AddHttpContextAccessor();

// --- Хранилище токена (простая кука) ---
builder.Services.AddScoped<TokenStorage>();

// --- Обработчик, добавляющий JWT в заголовки запросов ---
builder.Services.AddScoped<AuthMessageHandler>();

// --- Единый HttpClient с поддержкой аутентификации ---
builder.Services.AddScoped(sp =>
{
    var tokenStorage = sp.GetRequiredService<TokenStorage>();
    var handler = new AuthMessageHandler(tokenStorage)
    {
        InnerHandler = new HttpClientHandler()
    };
    var client = new HttpClient(handler)
    {
        BaseAddress = new Uri("https://localhost:7159/api/")
    };
    return client;
});

// --- API-клиент для удобства вызовов ---
builder.Services.AddScoped<ApiClient>();

// --- Провайдер состояния аутентификации (читает куку) ---
builder.Services.AddScoped<AuthenticationStateProvider, TokenAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore();

// --- Фиктивная схема, чтобы работал [Authorize] ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Custom";
    options.DefaultChallengeScheme = "Custom";
})
.AddScheme<AuthenticationSchemeOptions, CustomAuthenticationHandler>("Custom", null);

// --- Blazor + MudBlazor ---
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();