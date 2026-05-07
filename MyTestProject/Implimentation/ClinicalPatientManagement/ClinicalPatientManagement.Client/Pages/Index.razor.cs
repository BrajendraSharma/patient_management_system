using Microsoft.AspNetCore.Components;
using ClinicalPatientManagement.Client.Services;

namespace ClinicalPatientManagement.Client.Pages;

/// <summary>
/// Index page - landing page for unauthenticated users, redirect for authenticated users
/// </summary>
public partial class Index
{
    [Inject]
    private IAuthStateService? AuthStateService { get; set; }

    [Inject]
    private NavigationManager? Navigation { get; set; }

    private bool shouldShowDashboard = false;

    protected override async Task OnInitializedAsync()
    {
        if (AuthStateService != null)
        {
            var isAuthenticated = await AuthStateService.IsAuthenticatedAsync();
            if (isAuthenticated)
            {
                shouldShowDashboard = true;
                Navigation?.NavigateTo("/dashboard", replace: true);
            }
        }
    }
}

