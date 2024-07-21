using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Singularity.Services;
using CommunityToolkit.Maui.Core.Primitives;

namespace Singularity.Components.Views;

public partial class TabBarContentView : IDisposable
{

    [Parameter]
    public string? Title { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public string Padding { get; set; } = "10px";

#nullable disable
    [Inject]
    public AudioManager AudioManager { get; set; }
#nullable restore

    [Parameter]
    public bool ShowSearchButton { get; set; } = false;
    
    [Parameter]
    public bool ShowAddButton { get; set; } = false;

    [Parameter]
    public bool ShowDeleteButton { get; set; } = false;
    [Parameter]
    public bool ShowCrossButton { get; set; } = false;
    [Parameter]
    public EventCallback CrossButtonClicked { get; set; }
    [Parameter]
    public EventCallback AddButtonClicked { get; set; }

    [Parameter]
    public EventCallback DeleteButtonClicked { get; set; }


    private bool IsSearchPage => Nav.Uri.EndsWith("search");

    private bool isMusicViewInitialized = false;
    private string lastSearch = string.Empty;

   

    [Parameter]
    public EventCallback<(string SearchTerm, bool Finalized)> OnSearch { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        AudioManager.MediaPlayer.StateChanged += MediaPlayer_StateChanged;
        isMusicViewInitialized = AudioManager.MediaPlayer.CurrentState == MediaElementState.Playing;
        StateHasChanged(); 
    }

    private async void MediaPlayer_StateChanged(object? sender, CommunityToolkit.Maui.Core.Primitives.MediaStateChangedEventArgs e)
    {
        await InvokeAsync(() =>
        {
            isMusicViewInitialized = true;
            StateHasChanged();
        });
    }

    public void UpdateLastSearch(string newsearch)
    {
        lastSearch = newsearch;
        StateHasChanged();
    }

    private void GoToSearch()
    {
        if (!IsSearchPage)
            Nav.NavigateTo("/search");
    }

    private void OnSearchChanged(ChangeEventArgs e)
    {
        lastSearch = e.Value!.ToString()!;
        OnSearch.InvokeAsync((lastSearch, false)!);
    }
    private void OnSearchFinished(KeyboardEventArgs e)
    {
        if (e.Code == "Enter")
            OnSearch.InvokeAsync((lastSearch, true)!);
    }

    public void Dispose()
    {
        AudioManager.MediaPlayer.StateChanged -= MediaPlayer_StateChanged;
    }
}
