using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Singularity.Components.Views;

namespace Singularity.Components.Pages;

public partial class UserPlaylistPage
{
    private bool addPlaylistNewVisible = false;
    private CreateNewPlaylistView? NewPlaylistElement;
    private async Task OnPlusButtonClicked()
    {
        NewPlaylistElement.SetOpen();
        StateHasChanged();
    }
}
