using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Singularity.Components.Views;

namespace Singularity.Components.Pages;

public partial class UserPlaylistGroupPage
{
    private bool addPlaylistNewVisible = false;
    private CreateNewPlaylistView? NewPlaylistElement;

    private void OnPlusButtonClicked()
    {
        NewPlaylistElement?.SetOpen();
        StateHasChanged();
    }
}
