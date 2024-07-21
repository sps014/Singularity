using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Singularity.Data;

namespace Singularity.Components.Views;

public partial class CreateNewPlaylistView
{
    private string? playlistName;

    [Parameter]
    public bool IsVisible { get; set; }

    [Parameter]
    public EventCallback<string> PlaylistCreated { get; set; }

    public void SetOpen()
    {
        IsVisible = true;
        StateHasChanged();
    }
    void ToggleAddPlayList()
    {
        IsVisible = false;
        StateHasChanged();
    }
    async Task CreateNewPlaylist()
    {
        if (string.IsNullOrWhiteSpace(playlistName))
            return;


        if(PlaylistSettings.Current.IsPlaylistExist(playlistName))
        {
            IsVisible = false;
            StateHasChanged();
            await PlaylistCreated.InvokeAsync(playlistName);
            return;
        }
        await PlaylistSettings.Current.CreateNewPlaylistAsync(playlistName);
        IsVisible = false;
        StateHasChanged();
        await PlaylistCreated.InvokeAsync(playlistName);

    }
}
