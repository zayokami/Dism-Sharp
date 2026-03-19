using Wpf.Ui.Abstractions.Controls;
using DismSharp.UI.ViewModels;

namespace DismSharp.UI.Views;

public partial class Win11FeaturesPage : INavigationAware
{
    public Win11FeaturesViewModel ViewModel { get; }

    public Win11FeaturesPage()
    {
        ViewModel = new Win11FeaturesViewModel();
        DataContext = ViewModel;
        InitializeComponent();
    }

    public async Task OnNavigatedToAsync()
    {
        await ViewModel.LoadDataCommand.ExecuteAsync(null);
    }

    public Task OnNavigatedFromAsync()
    {
        return Task.CompletedTask;
    }
}
