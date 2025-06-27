using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using HRKošarka.UI.Services.Base.Common.Requests;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace HRKošarka.UI.Components.Pages.Club
{
    public partial class Clubs : ComponentBase
    {
        [Inject] public IClubService ClubService { get; set; }
        [Inject] public AuthenticationStateProvider AuthStateProvider { get; set; }
        [Inject] public ISnackbar Snackbar { get; set; }

        protected string _searchTerm = "";
        protected List<ClubDTO> _clubs = new();
        protected bool _loading = false;

        protected bool _canCreate = false;
        protected bool _canEdit = false;
        protected bool _canDeactivate = false;
        protected bool _canDelete = false;

        protected override async Task OnInitializedAsync()
        {
            await SetRolePermissions();
        }

        private async Task SetRolePermissions()
        {
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            _canCreate = user.IsInRole("Administrator");
            _canEdit = user.IsInRole("Administrator") || user.IsInRole("ClubManager");
            _canDeactivate = _canEdit;
            _canDelete = user.IsInRole("Administrator");
        }

        public async Task<TableData<ClubDTO>> LoadServerData(TableState state, CancellationToken token)
        {
            _loading = true;

            var request = new PaginationRequest
            {
                Page = state.Page + 1,
                PageSize = state.PageSize,
                SortBy = state.SortLabel ?? "Name",
                SortDirection = state.SortDirection == SortDirection.Descending ? "desc" : "asc",
                SearchTerm = _searchTerm
            };

            var response = await ClubService.GetClubs(request);

            _clubs = response.Data?.ToList() ?? new List<ClubDTO>();
            _loading = false;

            return new TableData<ClubDTO>
            {
                Items = _clubs,
                TotalItems = response.Pagination?.TotalCount ?? _clubs.Count
            };
        }


        public void ReloadClubs()
        {
            StateHasChanged();
        }

        public void CreateClub()
        {
            Snackbar.Add("Create Club clicked (implement navigation)", Severity.Info);
        }

        public void EditClub(Guid id)
        {
            Snackbar.Add($"Edit Club {id} clicked (implement navigation)", Severity.Info);
        }

        public void ViewClub(Guid id)
        {
            Snackbar.Add($"View Club {id} clicked (implement navigation)", Severity.Info);
        }

        public async Task DeactivateClub(Guid id)
        {
            var result = await ClubService.DeactivateClub(id);
            if (result.IsSuccess)
                Snackbar.Add("Club deactivated.", Severity.Success);
            else
                Snackbar.Add(result.Message, Severity.Error);

            ReloadClubs();
        }

        public async Task DeleteClub(Guid id)
        {
            var result = await ClubService.DeleteClub(id);
            if (result.IsSuccess)
                Snackbar.Add("Club deleted.", Severity.Success);
            else
                Snackbar.Add(result.Message, Severity.Error);

            ReloadClubs();
        }
    }
}
