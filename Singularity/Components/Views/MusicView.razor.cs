using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorBindGen;
using CommunityToolkit.Maui.Core.Primitives;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Singularity.Services;

namespace Singularity.Components.Views;

public  partial class MusicView
{
    public static MusicView? Instance { get; private set; }
    
    private bool isExpanded = false;
    public bool IsExpanded
    {
        get
        {
            return isExpanded;
        }
        set
        {
            isExpanded = value;
            StateHasChanged();
        }
    }
    protected override void OnInitialized()
    {
        base.OnInitialized();
        Instance = this;
    }
    private void ToggleMusicView()
    {
        IsExpanded = !IsExpanded;
    }
}
