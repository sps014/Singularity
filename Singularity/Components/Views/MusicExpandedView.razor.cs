using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Singularity.Services;

namespace Singularity.Components.Views;

public partial class MusicExpandedView
{
#nullable disable
    [Inject]
    public AudioManager AudioManager { get; set; }
#nullable restore

    [Parameter]
    public EventCallback OnToggled { get; set; }
}
