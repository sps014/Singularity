using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Singularity.Context
{
    internal class LongPressItemContext
    {
        public DotNetObjectReference<LongPressItemContext> Dotnet { get; }

        public required Action LongPressCallbackAction { get; set; }

        [DynamicDependency(nameof(LongPressedInternal))]
        public LongPressItemContext()
        {
            Dotnet = DotNetObjectReference.Create(this);
        }

        [JSInvokable("longPress")]
        public void LongPressedInternal()
        {
            LongPressCallbackAction?.Invoke();
        }
    }
}
