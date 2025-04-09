using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FFEC;
using FFECAvalonia.Views.DialogView;
using System.Threading.Tasks;
namespace FFECAvalonia.ViewModels;

public partial class LayoutsPageViewModel : ViewModelBase
{
    public LayoutsPageViewModel()
    {
        _configItems = new ObservableCollection<string>(Config.GetList());
        _currentConfigItem = JsonStorage.Config["Configuration"].Value<string>();
    }
    [ObservableProperty]
    private ObservableCollection<string> _configItems;

    [ObservableProperty]
    private string _selectedConfigItem;

    [ObservableProperty]
    private string _currentConfigItem;



    [RelayCommand]
    public void Set(string item = null)
    {
        JsonStorage.Config["Configuration"] = item ?? SelectedConfigItem;
        JsonStreamer.WriteConfig(JsonStorage.Config);
        CurrentConfigItem = Global.currentConfigName = JsonStorage.Config["Configuration"].Value<string>();
    }

    [RelayCommand]
    public async void Add()
    {
        string? result = await Global.ShowDialogAsync();
        if (result is not null)
        {
            Config.Set(result, JsonStorage.Config["DefaultConfiguration"].ToObject<JObject>());

            JsonStreamer.WriteConfigurations(JsonStorage.Configurations);
            ConfigItems = new ObservableCollection<string>(Config.GetList());
        }
    }

    [RelayCommand]
    public void Delete()
    {
        if (Global.currentConfigName == SelectedConfigItem) return;
        Config.Remove(SelectedConfigItem);
        JsonStreamer.WriteConfigurations(JsonStorage.Configurations);
        ConfigItems = new ObservableCollection<string>(Config.GetList());
    }
}