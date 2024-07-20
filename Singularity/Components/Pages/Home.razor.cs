using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
using Microsoft.AspNetCore.Components;
using Singularity.Components.Layout;
using Singularity.Contracts;
using Singularity.Services;

namespace Singularity.Components.Pages;

public partial class Home 
{
    protected override async Task OnInitializedAsync()
    {
        //var user = await AuthService.LoginUserAsync("abcd@gmail.com", "123456");
        //await AuthService.LogoutUserAsync();
        if (!MainLayout.UserAuthStateRead)
            return;

        var table = await DbService.GetTableAsync("test");
        await DbService.DeleteTableAsync("test");
        var data = await table.ToAsync<Data>();
    }
    record Data(int a,int b);


}
