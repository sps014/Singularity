using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Singularity.Contracts;
using YoutubeExplode.Playlists;

namespace Singularity.Components.Pages;

public partial class OnlinePlaylistPage
{

#nullable disable

    [Parameter]
    public string Id { get; set; }

    IMusicPlaylist playlist;

#nullable enable

    public IAsyncEnumerable<ISong>? Songs;

    protected override async Task OnInitializedAsync()
    {
        if (Id == null)
            return;

        playlist = await MusicHub.GetPlaylistAsync(Id);
        if(playlist == null) return;
        Songs = playlist.GetAsync();
        StateHasChanged();
    }
}
