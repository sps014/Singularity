using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorBindGen;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Singularity.Contracts;

namespace Singularity.Components.Views;

public partial class SmartMusicListView : ComponentBase, IAsyncDisposable
{
    [Parameter]
    public IAsyncEnumerable<ISong>? Songs { get; set; }

    [Parameter]
    public int ViewItemCount { get; set; } = 15;

    public List<ISong> ResizableSongsList = new List<ISong>();

    private ElementReference endElement;

    private DotNetObjectReference<SmartMusicListView>? dotnetObjectReference;

    private bool isFinishedLoading = false;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        dotnetObjectReference = DotNetObjectReference.Create(this);

        if (Songs == null)
            return;

        await AddNextSongBatch();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender)
            return;

        await BindGen.Window.CallVoidAsync("subscribeObserver", endElement,dotnetObjectReference!);
    }

    private async ValueTask AddNextSongBatch()
    {
        if (Songs == null)
            return;

        int start = ResizableSongsList.Count;
        int end = ResizableSongsList.Count+ViewItemCount;

        int ct = 0;
        await foreach (var song in Songs)
        {
            if (start >= end)
                break;

            start++;
            ct++;
            ResizableSongsList.Add(song);
        }
        isFinishedLoading = ct == 0 || ct!=ViewItemCount;
        StateHasChanged();
    }

    public async ValueTask DisposeAsync()
    {
        await BindGen.Window.CallVoidAsync("unsubscribeObserver");
        dotnetObjectReference?.Dispose();
    }

    [JSInvokable("visibiltyChanged")]
    public async void VisibiltyChanged(bool visible)
    {
        if (!visible)
            return;
        await AddNextSongBatch();
    }
}
