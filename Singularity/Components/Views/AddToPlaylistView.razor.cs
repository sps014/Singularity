using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Singularity.Components.Views;

public partial class AddToPlaylistView
{
    private bool isActive = false;

    private void OnClose()
    {
        isActive = false;
        StateHasChanged();
    }
    public void Show()
    {
        isActive = true;
        StateHasChanged();
    }
}