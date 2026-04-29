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
        set => _ = value; 
    }

    public void Clear()
    {
       
    }
}