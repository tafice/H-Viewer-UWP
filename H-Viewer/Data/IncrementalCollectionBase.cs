using HViewer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace HViewer.Data
{
    public class IncrementalCollectionBase<T> : ObservableCollection<T>, ISupportIncrementalLoading, INotifyPropertyChanged
    {
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            throw new Exception();
        }

        public bool HasMoreItems { get; set; }
    }
}
