using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.Components.Pages;

public partial class UserPlaylistPage
{
    private bool addPlaylistNewVisible = false;

    private async Task OnPlusButtonClicked()
    {
        addPlaylistNewVisible=true;
        StateHasChanged();
    }
}
