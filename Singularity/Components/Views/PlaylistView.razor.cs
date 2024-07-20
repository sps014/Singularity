using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Singularity.Data;

namespace Singularity.Components.Views;

public partial class PlaylistView : IDisposable
{
    protected override void OnInitialized()
    {
        PlaylistSettings.Current.PlaylistUpdated += CurrentPlaylistUpdated;
    }

    private void CurrentPlaylistUpdated(object? sender, EventArgs e)
    {
        StateHasChanged();
    }

    public void Dispose()
    {
        PlaylistSettings.Current.PlaylistUpdated -= CurrentPlaylistUpdated;
    }

}
