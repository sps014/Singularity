using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Singularity.Contracts;

namespace Singularity.Components.Views;

public partial class SmartMusicListView : ComponentBase
{
    [Parameter]
    public IAsyncEnumerable<ISong>? Songs { get; set; }

    [Parameter]
    public int ViewItemCount { get; set; } = 15;

    public List<ISong> ResizableSongsList = new List<ISong>();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (Songs == null)
            return;

        await AddNextSongBatch();
    }

    private async ValueTask AddNextSongBatch()
    {
        if (Songs == null)
            return;

        int start = ResizableSongsList.Count;
        int end = ResizableSongsList.Count+ViewItemCount;


        await foreach (var song in Songs)
        {
            if (start > end)
                break;

            start++;
            ResizableSongsList.Add(song);
        }

        StateHasChanged();
    }

}
