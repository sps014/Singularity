using Singularity.Services;

namespace Singularity
{
    public partial class MainPage : ContentPage
    {
#nullable disable
        public static MainPage Current { get; private set; }
#nullable restore

        public MainPage()
        {
            InitializeComponent();
            Current = this;
            AudioManager.InitMediaElement(mediaElement);
        }
    }
}
