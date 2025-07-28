using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace HRKošarka.UI.Components.Shared
{
    public partial class MainLayout : LayoutComponentBase
    {
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        private MudThemeProvider _mudThemeProvider = default!;
        private bool _drawerOpen = true;
        private bool _isDarkMode = true;
        private MudTheme _currentTheme = new();

        protected override async Task OnInitializedAsync()
        {
            _currentTheme = CreateModernDarkTheme();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
                StateHasChanged();
            }
        }

        private MudTheme CreateModernDarkTheme()
        {
            return new MudTheme
            {
                PaletteLight = new PaletteLight
                {
                    Black = "#0d1117",
                    White = "#ffffff",
                    Primary = "#58a6ff",
                    Secondary = "#f78166",
                    Tertiary = "#56d364",
                    Info = "#79c0ff",
                    Success = "#56d364",
                    Warning = "#d29922",
                    Error = "#f85149",
                    Dark = "#161b22",
                    Background = "#0d1117",
                    BackgroundGray = "#161b22",
                    Surface = "#21262d",
                    DrawerBackground = "#161b22",
                    DrawerText = "#f0f6fc",
                    AppbarBackground = "#161b22",
                    AppbarText = "#f0f6fc",
                    TextPrimary = "#f0f6fc",
                    TextSecondary = "#8b949e",
                    ActionDefault = "#f0f6fc",
                    ActionDisabled = "#484f58",
                    ActionDisabledBackground = "#21262d",
                    Divider = "#30363d",
                    DividerLight = "#21262d",
                    TableLines = "#30363d",
                    LinesDefault = "#30363d",
                    LinesInputs = "#30363d",
                    GrayDefault = "#484f58",
                    GrayLight = "#6e7681",
                    GrayLighter = "#8b949e",
                    GrayDark = "#21262d",
                    GrayDarker = "#161b22",
                    OverlayDark = "rgba(13, 17, 23, 0.8)",
                    OverlayLight = "rgba(240, 246, 252, 0.05)"
                },
                PaletteDark = new PaletteDark
                {
                    Black = "#0d1117",
                    White = "#ffffff",
                    Primary = "#58a6ff",
                    Secondary = "#f78166",
                    Tertiary = "#56d364",
                    Info = "#79c0ff",
                    Success = "#56d364",
                    Warning = "#d29922",
                    Error = "#f85149",
                    Dark = "#0d1117",
                    Background = "#0d1117",
                    BackgroundGray = "#161b22",
                    Surface = "#21262d",
                    DrawerBackground = "#161b22",
                    DrawerText = "#f0f6fc",
                    AppbarBackground = "#161b22",
                    AppbarText = "#f0f6fc",
                    TextPrimary = "#f0f6fc",
                    TextSecondary = "#8b949e",
                    ActionDefault = "#f0f6fc",
                    ActionDisabled = "#484f58",
                    ActionDisabledBackground = "#21262d",
                    Divider = "#30363d",
                    DividerLight = "#21262d",
                    TableLines = "#30363d",
                    LinesDefault = "#30363d",
                    LinesInputs = "#30363d",
                    GrayDefault = "#484f58",
                    GrayLight = "#6e7681",
                    GrayLighter = "#8b949e",
                    GrayDark = "#21262d",
                    GrayDarker = "#161b22",
                    OverlayDark = "rgba(13, 17, 23, 0.8)",
                    OverlayLight = "rgba(240, 246, 252, 0.05)"
                }
            };
        }

        void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }
    }
}
