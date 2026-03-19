using CommunityToolkit.Mvvm.ComponentModel;

namespace DismSharp.UI.Helpers;

public partial class SelectableItem<T> : ObservableObject where T : class
{
    [ObservableProperty]
    private bool _isSelected;

    public T Item { get; }

    public SelectableItem(T item)
    {
        Item = item;
    }
}
