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
using Singularity.Models;
using Singularity.Services;

namespace Singularity.Components.Pages;

public partial class Home 
{
    private ExploreItem[]? ExploreItems;
    private const string TopChartPlaylistId = "PLPit0qsdr5cfYbW72VPdIX_2DRYUDJDGY";

    protected override async Task OnInitializedAsync()
    {
        if (!MainLayout.UserAuthStateRead)
            return;

        ExploreItems = await ExploreItem.LoadAsync();
        StateHasChanged();
        await UserSettings.LoadSettingsFromDb(DbService);
    }

    private void OpenExploreItem(ExploreItem item)
    {
        Nav.NavigateTo(GetPlaylistUrl(item.PlaylistId));
    }
    private string GetPlaylistUrl(string id)
    {
        return $"/youtubePlaylist/{id}";
    }
    private void OpenTopCharts()
    {
        Nav.NavigateTo(GetPlaylistUrl(TopChartPlaylistId));
    }
}
