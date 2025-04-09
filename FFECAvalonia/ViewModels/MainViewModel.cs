﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
namespace FFECAvalonia.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        _isPaneOpen = false;
        _currentPage = new CalculatorPageViewModel();
        _items = [
            new ListItemTemplate(typeof(CalculatorPageViewModel), iconKey: "calculator"),
            new ListItemTemplate(typeof(LayoutsPageViewModel), iconKey: "layouts"),
            // new ListItemTemplate(typeof(VariablesPageViewModel), iconKey: "variables"),
            // new ListItemTemplate(typeof(TutorialPageViewModel), iconKey: "tutorial"),
            new ListItemTemplate(typeof(TableSizePageViewModel), iconKey: "table"),
            new ListItemTemplate(typeof(AboutPageViewModel), iconKey: "about")
        ];
        _selectedListItem = Items[0];
    }

    #region SplitView

    [ObservableProperty]
    private bool _isPaneOpen;

    [ObservableProperty]
    private ViewModelBase _currentPage;

    [ObservableProperty]
    private ObservableCollection<ListItemTemplate> _items;

    [ObservableProperty]
    private ListItemTemplate? _selectedListItem;

    [RelayCommand]
    private void TriggerPane()
    {
        IsPaneOpen = !IsPaneOpen;
    }

    partial void OnSelectedListItemChanged(ListItemTemplate? value)
    {
        if (value == null) return;
        var instance = Activator.CreateInstance(value.ModelType);
        if (instance == null) return;
        CurrentPage = (ViewModelBase)instance;
    }

    #endregion
}

public class ListItemTemplate
{
    public ListItemTemplate (Type type, string iconKey)
    {
        ModelType = type;
        Label = type.Name.Replace("PageViewModel", "");

        Application.Current!.TryFindResource(iconKey, out var resource);
        ListItemIcon = (StreamGeometry)resource!;
    }

    public String Label {get;}
    public Type ModelType {get;}
    public StreamGeometry ListItemIcon { get; }
}