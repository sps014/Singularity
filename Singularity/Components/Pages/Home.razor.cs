using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
using Microsoft.AspNetCore.Components;
using Singularity.Components.Layout;
using Singularity.Contracts;
using Singularity.Data;
using Singularity.Services;

namespace Singularity.Components.Pages;

public partial class Home 
{
    protected override async Task OnInitializedAsync()
    {
        if (!MainLayout.UserAuthStateRead)
            return;

        await UserSettings.LoadSettingsFromDb(DbService);
    }
}
