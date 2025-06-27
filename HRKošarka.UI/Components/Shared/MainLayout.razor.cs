using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace HRKošarka.UI.Components.Shared
{
    public partial class MainLayout
    {
        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        bool _drawerOpen = true;
        private bool _hasRendered = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _hasRendered = true;
                var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
                StateHasChanged();
            }
        }

        void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }
    }
}
