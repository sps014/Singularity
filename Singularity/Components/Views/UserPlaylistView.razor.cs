using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Singularity.Data;

namespace Singularity.Components.Views;

public partial class UserPlaylistView : IDisposable
{
    [Parameter]
    public EventCallback<string> OnPlaylistSelected { get; set; }
    protected override void OnInitialized()
    {
        PlaylistSettings.Current.PlaylistUpdated += CurrentPlaylistUpdated;
    }

    private void CurrentPlaylistUpdated(object? sender, EventArgs e)
    {
        StateHasChanged();
    }

    private void PlaylistSelected(string id)
    {
        Nav.NavigateTo("/userPlaylistExpandedPage/"+id);
    }

    public void Dispose()
    {
        PlaylistSettings.Current.PlaylistUpdated -= CurrentPlaylistUpdated;
    }

}
