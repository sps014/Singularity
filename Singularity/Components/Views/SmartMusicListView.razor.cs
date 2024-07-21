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
    public bool CanPlay { get; set; } = true;

    [Parameter]
    public bool IsLongPressable { get; set; }

    [Parameter]
    public EventCallback<string>  OnLongPressed { get; set; }

    [Parameter]
    public IAsyncEnumerable<ISong>? Songs { get; set; }

    [Parameter]
    public int ViewItemCount { get; set; } = 15;

    public List<ISong> ResizableSongsList = new List<ISong>();

    private ElementReference endElement;

    private DotNetObjectReference<SmartMusicListView>? dotnetObjectReference;

    private bool isFinishedLoading = false;

    private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1,1);

    private string? currentSelectedId;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        dotnetObjectReference = DotNetObjectReference.Create(this);

        if (Songs == null)
            return;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender)
            return;

        await BindGen.Window.CallVoidAsync("subscribeObserver", endElement,dotnetObjectReference!);
    }

    private void TrackLongPressed(ISong song)
    {
        if (!IsLongPressable)
            return;

        currentSelectedId = song.Id;
        CanPlay = false;
        StateHasChanged();
        OnLongPressed.InvokeAsync(song.Id);
    }

    public void ResetCurrentSelectionInList()
    {
        currentSelectedId = null;
        CanPlay = true;
        StateHasChanged();
    }
    private async ValueTask AddNextSongBatch()
    {
        if (Songs == null)
            return;

        await semaphoreSlim.WaitAsync();
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
        semaphoreSlim.Release();
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

    internal void RemoveSongFromList(string selectedSongId)
    {
        var item =ResizableSongsList.FirstOrDefault(x => x.Id == selectedSongId);
        if(item != null)
            ResizableSongsList.Remove(item);
        StateHasChanged();
    }
}
