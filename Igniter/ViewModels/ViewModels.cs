namespace Igniter.ViewModel
{
    public sealed class ViewModels
    {
        public static readonly ViewModels Instance = IgniterComponent.GetBuiltInData<ViewModels>(nameof(ViewModels));

        public MainViewModel MainValue { get; } = new MainViewModel();
    }
}