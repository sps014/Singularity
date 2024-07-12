using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;

namespace Singularity.Components.Views;

public partial class SearchBarPageView
{

    [Parameter]
    public string? Title { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public string Padding { get; set; } = "10px";


    private bool IsSearchPage => Nav.Uri.EndsWith("search");

    private bool isMusicViewInitialized = false;
    private string lastSearch = string.Empty;

    [Parameter]
    public EventCallback<(string SearchTerm, bool Finalized)> OnSearch { get; set; }


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
}
