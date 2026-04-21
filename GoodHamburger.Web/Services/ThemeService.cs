using Microsoft.JSInterop;

namespace GoodHamburger.Web.Services;

public class ThemeService : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private const string ThemeKey = "theme_preference";
    private bool _initialized;
    private DotNetObjectReference<ThemeService>? _dotNetRef; // allows to JS call .NET code

    public bool IsDarkMode { get; private set; }
    public event Action? OnThemeChanged;

    public ThemeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task InitializeAsync()
    {
        if (_initialized) return;
        
        try
        {
            var savedTheme = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", ThemeKey);
            
            if (string.IsNullOrEmpty(savedTheme) || savedTheme == "system")
            {
                IsDarkMode = await _jsRuntime.InvokeAsync<bool>("window.themeManager.isDarkMode")
                    .ConfigureAwait(false);
            }
            else
            {
                IsDarkMode = savedTheme == "dark";
            }

            _dotNetRef = DotNetObjectReference.Create(this);
            await _jsRuntime.InvokeVoidAsync("window.themeManager.init", _dotNetRef);
        }
        catch
        {
            IsDarkMode = false;
        }
        
        _initialized = true;
        OnThemeChanged?.Invoke();
    }

    [JSInvokable]
    public void SetThemeFromSystem(bool isDarkMode)
    {
        IsDarkMode = isDarkMode;
        OnThemeChanged?.Invoke();
    }

    public async Task ToggleThemeAsync()
    {
        IsDarkMode = !IsDarkMode;
        await SaveThemePreferenceAsync();
        OnThemeChanged?.Invoke();
    }

    private async Task SaveThemePreferenceAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", ThemeKey, IsDarkMode ? "dark" : "light");
        }
        catch { }
    }

    public async ValueTask DisposeAsync()
    {
        if (_dotNetRef != null)
        {
            _dotNetRef.Dispose();
            _dotNetRef = null;
        }
        await ValueTask.CompletedTask;
    }
}
