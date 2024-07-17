using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Singularity.Components.Views;
using Singularity.Contracts;

namespace Singularity.Components.Pages;

public partial class SearchPage
{
    public bool IsInSearchTypeMode { get; private set; } = true;
    public ICollection<string> SearchSuggestions { get; private set; } = new List<string>();
    public ICollection<ISong> SearchedSongList { get; private set; } = new List<ISong>();
    public bool IsSearchComplete { get; private set; } = true;

#nullable disable
    [Inject]
    public IMusicHub MusicHub { get; set; }

    private SearchBarPageView tabView;
#nullable restore

    private CancellationTokenSource searchCancellation = new CancellationTokenSource();
    private static SemaphoreSlim SearchSemaphore = new SemaphoreSlim(1, 1);


    private void OnLinkClicked(string option)
    {
        tabView.UpdateLastSearch(option);
        OnSearch((option, true));
    }

    private async void OnSearch((string Query, bool SearchFinished) e)
    {

        if (string.IsNullOrWhiteSpace(e.Query))
            return;

        IsInSearchTypeMode = !e.SearchFinished;

        if(IsInSearchTypeMode)
        {
            SearchSuggestions = await MusicHub.SuggestionsAsync(e.Query,searchCancellation);
            StateHasChanged();
            return;
        }


        if (SearchSemaphore.CurrentCount == 0)
        {
            //cancel previous search
            searchCancellation.Cancel();
        }
        await SearchSemaphore.WaitAsync();
        IsSearchComplete = false;

        SearchedSongList = await MusicHub.SearchAsync(e.Query,searchCancellation,15);

        SearchSemaphore.Release();
        IsSearchComplete = true;
        StateHasChanged();

    }
}
