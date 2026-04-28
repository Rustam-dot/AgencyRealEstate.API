namespace AgencyRealEstate.WebUI.Services;

public class TokenStorage
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenStorage(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? Token
    {
        get => _httpContextAccessor.HttpContext?.Request.Cookies["authToken"];
        set => _ = value; // кука устанавливается через JS в Login.razor
    }

    public void Clear()
    {
        // кука удаляется через JS в Logout.razor
    }
}