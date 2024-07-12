using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Core.Primitives;
using Microsoft.AspNetCore.Components;
using Singularity.Services;

namespace Singularity.Components.Views;

public  partial class MusicView
{
#nullable disable
    [Inject]
    public AudioManager AudioManager { get; set; }

#nullable restore
    protected override void OnInitialized()
    {
        base.OnInitialized();
        AudioManager.MediaPlayer.StateChanged += MediaPlayerStateChanged;
    }

    private async void MediaPlayerStateChanged(object? sender, MediaStateChangedEventArgs e)
    {
        await InvokeAsync(() =>
        {
            StateHasChanged();

        });
    }
}
