
using System.Collections.ObjectModel;

namespace FFECAvalonia.Models
{
    public class TabItem
    {
        public string Header { get; }
        
        public ObservableCollection<ControlItem> Content{ get; }

        public TabItem(string header, ObservableCollection<ControlItem> content)
        {
            Header = header;
            Content = content;
        }
    }
}