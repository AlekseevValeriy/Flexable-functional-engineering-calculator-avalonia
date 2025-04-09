using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FFEC;

namespace FFECAvalonia.ViewModels;

public partial class VariablesPageViewModel : ViewModelBase
{
    public VariablesPageViewModel()
    {
        try
        {
            foreach (KeyValuePair<string, int> pair in JsonStorage.Configurations[Global.currentConfigName]["Variables"].ToObject<Dictionary<string, int>>())
            {
                _variableItems.Add(new(pair));
            }
        }
        catch (Exception ex) {}
    }

    [ObservableProperty]
    private ObservableCollection<VariableItemTemplate> _variableItems = new();

    [ObservableProperty]
    private VariableItemTemplate _selectedItem;

    [RelayCommand]
    public async void Add()
    {
        string? result = await Global.ShowDialogAsync();
        if (result is not null)
        {
            JsonStorage.Configurations[Global.currentConfigName]["Variables"][result] = 0;
            JsonStreamer.WriteConfigurations(JsonStorage.Configurations);
            VariableItems.Add(new (new (result, 0)));
        }
    }

    [RelayCommand]
    public void Delete()
    {
        (JsonStorage.Configurations[Global.currentConfigName]["Variables"] as JObject).Remove(SelectedItem.Name);
        JsonStreamer.WriteConfigurations(JsonStorage.Configurations);
        VariableItems.Remove(SelectedItem);
    }
}

public class VariableItemTemplate
{
    public VariableItemTemplate (string name, string value)
    {
        Name = name;
        Value = value;
    }

    public VariableItemTemplate (KeyValuePair<string, int> pair)
     : this(pair.Key, pair.Value.ToString()) 
    {

    }

    public string Name {get;}
    public string Value {get; set;}
}