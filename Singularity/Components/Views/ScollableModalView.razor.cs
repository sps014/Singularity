using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Singularity.Components.Views;

public partial class ScollableModalView
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public string BackgroundColor { get; set; } = "rgba(60, 33, 146, 0.2)";

    [Parameter]
    public bool IsActive { get; set; }

    [Parameter]
    public EventCallback OnCloseButtonClick { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
    }
    public void Dispose()
    {
    }

    public void ClosePage()
    {
        IsActive = false;
        OnCloseButtonClick.InvokeAsync();
        StateHasChanged();
    }
    private void OnBeforeInternalNavigation(LocationChangingContext context)
    {

        if (IsActive)
        {
            IsActive = false;
            context.PreventNavigation();
            StateHasChanged();
        }
    }

}
