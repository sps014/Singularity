using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Routing;
using Singularity.Contracts;

namespace Singularity.Components.Layout;

public partial class MainLayout:IDisposable
{
    bool userAuthStateRead = false;
    IUser? user;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await AuthService.WatchAuthStateAsync();
        AuthService.OnAuthStateChanged += OnAuthStateChanged;
    }



    private void OnAuthStateChanged(object? sender, IUser? user)
    {
        userAuthStateRead = true;
        StateHasChanged();
        this.user = user;

        if (user == null)
            NavManager.NavigateTo("/login");
    }

    public void Dispose()
    {
        AuthService.OnAuthStateChanged -= OnAuthStateChanged;
    }

}
