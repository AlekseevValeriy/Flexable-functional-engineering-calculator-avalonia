using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FFEC;
using FFECAvalonia.Views.DialogView;
using System.Threading.Tasks;

namespace FFECAvalonia.ViewModels;

public partial class TableSizePageViewModel : ViewModelBase
{
    
    [RelayCommand]
    public void ButtonMatrixHeightUp()
    {
        JsonStorage.Configurations[Global.currentConfigName]["TableStructs"]["Controls"][1] = JsonStorage.Configurations[Global.currentConfigName]["TableStructs"]["Controls"][1].Value<int>() + 1;
        JsonStreamer.WriteConfigurations(JsonStorage.Configurations);
    }
    
    [RelayCommand]
    public void ButtonMatrixHeightDown()
    {
        JsonStorage.Configurations[Global.currentConfigName]["TableStructs"]["Controls"][1] = JsonStorage.Configurations[Global.currentConfigName]["TableStructs"]["Controls"][1].Value<int>() - 1;
        JsonStreamer.WriteConfigurations(JsonStorage.Configurations);
    }
    
    [RelayCommand]
    public void ButtonMatrixWidthUp()
    {
        JsonStorage.Configurations[Global.currentConfigName]["TableStructs"]["Controls"][0] = JsonStorage.Configurations[Global.currentConfigName]["TableStructs"]["Controls"][0].Value<int>() + 1;
        JsonStreamer.WriteConfigurations(JsonStorage.Configurations);
    }
    
    [RelayCommand]
    public void ButtonMatrixWidthDown()
    {
        JsonStorage.Configurations[Global.currentConfigName]["TableStructs"]["Controls"][0] = JsonStorage.Configurations[Global.currentConfigName]["TableStructs"]["Controls"][0].Value<int>() - 1;
        JsonStreamer.WriteConfigurations(JsonStorage.Configurations);
    }
}