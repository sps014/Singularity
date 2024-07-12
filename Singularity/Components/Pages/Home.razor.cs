using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
using Microsoft.AspNetCore.Components;
using Singularity.Contracts;
using Singularity.Services;

namespace Singularity.Components.Pages;

public partial class Home
{

#nullable disable
    [Inject]
    public AudioManager AudioManager { get; set; }
    [Inject]
    public IMusicHub MusicHub { get; set; }

#nullable restore

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        var song = await MusicHub!.GetSongMetaDataAsync("AMuRRXCuy-4");

        if (song == null)
            return;

        await AudioManager.AddSongAsync(song);
        await AudioManager.PlayAsync();

    }
}
