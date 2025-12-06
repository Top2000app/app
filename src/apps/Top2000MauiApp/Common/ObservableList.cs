using System.Collections.Specialized;

namespace Top2000MauiApp.Common;

public class ObservableList<TItem> : List<TItem>, INotifyCollectionChanged
{
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <summary>
    /// Removed all the items in the list, add the list of items and notify the observers.
    /// </summary>
    /// <param name="items">Items to add</param>
    public void ClearAddRange(IEnumerable<TItem> items)
    {
        this.Clear();
        items.ToList().ForEach(this.Add);

        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
}
