using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Input;
using Avalonia.Layout;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FFECAvalonia.CustomControls;
using FFECAvalonia.Models;
using FFECAvalonia.Views;
using Avalonia.Interactivity;
using FFEC;
using FFEC.ExpressionModule.Expression;
using Avalonia.Controls.Shapes;
using System.Threading.Tasks;
using Avalonia.Platform;
using Avalonia.Threading;

namespace FFECAvalonia.ViewModels;

public partial class CalculatorPageViewModel : ViewModelBase
{
    private const ushort FCHeightMax = 300; 


    public CalculatorPageViewModel()
    {
        // ToolsArea
        _controlsIsChecked = true;
        _layoutsIsChecked = true;

        // LayoutArea
        _isPaneLPOpen = false;
        _layoutItems = [
            "Base",
            "Special",
            "Standard"
        ];
        
        // CatalogArea
        _isPanelFCOpen = false;
        _panelFCHeight = 0;
        _catalogGridWidth = 3;


        _displayTabMatrix = new (1, _catalogGridWidth, "DisplayCatalog");
        _displayTabMatrix.CollectionIntegrate( new List<ControlItem>()
        {
            new ControlItem() {Position = new (){{"x", 0}, {"y", 0}}}
        });
        _displayTabMatrixItems = _displayTabMatrix.ToCollection();


        _numbersTabMatrix = new (4, _catalogGridWidth, "NumbersCatalog");
        _numbersTabMatrix.CollectionIntegrate( new List<ControlItem>()
        {
            new ControlItem() {Sector = "Numbers", Name="Zero", Text = "0", Position = new (){{"x", 0}, {"y", 0}}},
            new ControlItem() {Sector = "Numbers", Name="One", Text = "1", Position = new (){{"x", 1}, {"y", 0}}},
            new ControlItem() {Sector = "Numbers", Name="Two", Text = "2", Position = new (){{"x", 2}, {"y", 0}}},
            new ControlItem() {Sector = "Numbers", Name="Three", Text = "3", Position = new (){{"x", 0}, {"y", 1}}},
            new ControlItem() {Sector = "Numbers", Name="Four", Text = "4", Position = new (){{"x", 1}, {"y", 1}}},
            new ControlItem() {Sector = "Numbers", Name="Five", Text = "5", Position = new (){{"x", 2}, {"y", 1}}},
            new ControlItem() {Sector = "Numbers", Name="Six", Text = "6", Position = new (){{"x", 0}, {"y", 2}}},
            new ControlItem() {Sector = "Numbers", Name="Seven", Text = "7", Position = new (){{"x", 1}, {"y", 2}}},
            new ControlItem() {Sector = "Numbers", Name="Eight", Text = "8", Position = new (){{"x", 2}, {"y", 2}}},
            new ControlItem() {Sector = "Numbers", Name="Nine", Text = "9", Position = new (){{"x", 0}, {"y", 3}}},
            new ControlItem() {Sector = "Numbers", Name="Pi", Text = "π", Position = new (){{"x", 1}, {"y", 3}}},
            new ControlItem() {Sector = "Numbers", Name="E", Text = "e", Position = new (){{"x", 2}, {"y", 3}}}
        });
        _numbersTabMatrixItems = _numbersTabMatrix.ToCollection();


        _actionsTabMatrix = new (3, _catalogGridWidth, "ActionsCatalog");
        _actionsTabMatrix.CollectionIntegrate( new List<ControlItem>()
        {
            new ControlItem() {Sector = "Actions", Name="ToDouble", Text = ",", Position = new (){{"x", 0}, {"y", 0}}},
            new ControlItem() {Sector = "Actions", Name="Backspace", Text = "←", Position = new (){{"x", 1}, {"y", 0}}},
            new ControlItem() {Sector = "Actions", Name="Equally", Text = "=", Position = new (){{"x", 2}, {"y", 0}}},
            new ControlItem() {Sector = "Actions", Name="Clear", Text = "C", Position = new (){{"x", 0}, {"y", 1}}},
            new ControlItem() {Sector = "Actions", Name="ClearElement", Text = "CE", Position = new (){{"x", 1}, {"y", 1}}},
            new ControlItem() {Sector = "Actions", Name="ChangeSign", Text = "±", Position = new (){{"x", 2}, {"y", 1}}},
            new ControlItem() {Sector = "Actions", Name="Parenthesis", Text = "( )", Position = new (){{"x", 0}, {"y", 2}}},
            new ControlItem() {Sector = "Actions", Name="CloseFunctionWrite", Text = "⟴", Position = new (){{"x", 1}, {"y", 2}}},
        });
        _actionsTabMatrixItems = _actionsTabMatrix.ToCollection();


        _operatorsTabMatrix = new (2, _catalogGridWidth, "OperatorsCatalog");
        _operatorsTabMatrix.CollectionIntegrate( new List<ControlItem>()
        {
            new ControlItem() {Sector = "Operators", Name="Append", Text = "+", Position = new (){{"x", 0}, {"y", 0}}},
            new ControlItem() {Sector = "Operators", Name="Subtract", Text = "-", Position = new (){{"x", 1}, {"y", 0}}},
            new ControlItem() {Sector = "Operators", Name="Multiply", Text = "×", Position = new (){{"x", 2}, {"y", 0}}},
            new ControlItem() {Sector = "Operators", Name="Division", Text = "÷", Position = new (){{"x", 0}, {"y", 1}}},
            new ControlItem() {Sector = "Operators", Name="Modular", Text = "mod", Position = new (){{"x", 1}, {"y", 1}}},
            new ControlItem() {Sector = "Operators", Name="DeleteElement", Text = "Delete", Position = new (){{"x", 2}, {"y", 1}}}
        });
        _operatorsTabMatrixItems = _operatorsTabMatrix.ToCollection();


        _functionTabMatrix = new (6, _catalogGridWidth, "FunctionCatalog");
        _functionTabMatrix.CollectionIntegrate( new List<ControlItem>()
        {
            new ControlItem() {Sector = "Function", Name="NaturalLogarithm", Text = "ln", Position = new (){{"x", 0}, {"y", 0}}},
            new ControlItem() {Sector = "Function", Name="EPowerOfX", Text = "eᵡ", Position = new (){{"x", 1}, {"y", 0}}},
            new ControlItem() {Sector = "Function", Name="DecimalLogarithm", Text = "log", Position = new (){{"x", 2}, {"y", 0}}},
            new ControlItem() {Sector = "Function", Name="LogarithmOfXBasedOnY", Text = "logᵧx", Position = new (){{"x", 0}, {"y", 1}}},
            new ControlItem() {Sector = "Function", Name="TenPowerOfX", Text = "10ᵡ", Position = new (){{"x", 1}, {"y", 1}}},
            new ControlItem() {Sector = "Function", Name="TwoPowerOfX", Text = "2ᵡ", Position = new (){{"x", 2}, {"y", 1}}},
            new ControlItem() {Sector = "Function", Name="XPowerOfY", Text = "xʸ", Position = new (){{"x", 0}, {"y", 2}}},
            new ControlItem() {Sector = "Function", Name="YRootOfX", Text = "ʸ√x", Position = new (){{"x", 1}, {"y", 2}}},
            new ControlItem() {Sector = "Function", Name="XPowerOfTwo", Text = "x²", Position = new (){{"x", 2}, {"y", 2}}},
            new ControlItem() {Sector = "Function", Name="XPowerOfThree", Text = "x³", Position = new (){{"x", 0}, {"y", 3}}},
            new ControlItem() {Sector = "Function", Name="SquareRootOfX", Text = "²√x", Position = new (){{"x", 1}, {"y", 3}}},
            new ControlItem() {Sector = "Function", Name="CubicRootOfX", Text = "³√x", Position = new (){{"x", 2}, {"y", 3}}},
            new ControlItem() {Sector = "Function", Name="XReverse", Text = "⅟ᵪ", Position = new (){{"x", 0}, {"y", 4}}},
            new ControlItem() {Sector = "Function", Name="XAbsolute", Text = "|x|", Position = new (){{"x", 1}, {"y", 4}}},
            new ControlItem() {Sector = "Function", Name="Exponential", Text = "exp", Position = new (){{"x", 2}, {"y", 4}}},
            new ControlItem() {Sector = "Function", Name="NFactorial", Text = "n!", Position = new (){{"x", 0}, {"y", 5}}},

        });
        _functionTabMatrixItems = _functionTabMatrix.ToCollection();


        _trigonometryTabMatrix = new (5, _catalogGridWidth, "TrigonometryCatalog");
        _trigonometryTabMatrix.CollectionIntegrate( new List<ControlItem>()
        {
            new ControlItem() {Sector = "Trigonometry", Name="Sine", Text = "sin", Position = new (){{"x", 0}, {"y", 0}}},
            new ControlItem() {Sector = "Trigonometry", Name="Cosine", Text = "cos", Position = new (){{"x", 1}, {"y", 0}}},
            new ControlItem() {Sector = "Trigonometry", Name="Tangent", Text = "tan", Position = new (){{"x", 2}, {"y", 0}}},
            new ControlItem() {Sector = "Trigonometry", Name="Cosecant", Text = "csc", Position = new (){{"x", 0}, {"y", 1}}},
            new ControlItem() {Sector = "Trigonometry", Name="Secant", Text = "sec", Position = new (){{"x", 1}, {"y", 1}}},
            new ControlItem() {Sector = "Trigonometry", Name="Cotangent", Text = "cot", Position = new (){{"x", 2}, {"y", 1}}},
            new ControlItem() {Sector = "Trigonometry", Name="Arcsine", Text = "arcsin", Position = new (){{"x", 0}, {"y", 2}}},
            new ControlItem() {Sector = "Trigonometry", Name="Arccosine", Text = "arccos", Position = new (){{"x", 1}, {"y", 2}}},
            new ControlItem() {Sector = "Trigonometry", Name="Arctangent", Text = "arctan", Position = new (){{"x", 2}, {"y", 2}}},
            new ControlItem() {Sector = "Trigonometry", Name="Arccosecant", Text = "arccsc", Position = new (){{"x", 0}, {"y", 3}}},
            new ControlItem() {Sector = "Trigonometry", Name="Arcsecant", Text = "arcsec", Position = new (){{"x", 1}, {"y", 3}}},
            new ControlItem() {Sector = "Trigonometry", Name="Arccotangent", Text = "arccot", Position = new (){{"x", 2}, {"y", 3}}},
            new ControlItem() {Sector = "Trigonometry", Name="DegreesTypeChange", Text = "DEG", Position = new (){{"x", 0}, {"y", 4}}},
            new ControlItem() {Sector = "Trigonometry", Name="ToDegreesMinutesSeconds", Text = "➞dms", Position = new (){{"x", 1}, {"y", 4}}},
            new ControlItem() {Sector = "Trigonometry", Name="ToDegrees", Text = "➞deg", Position = new (){{"x", 2}, {"y", 4}}},
        });
        _trigonometryTabMatrixItems = _trigonometryTabMatrix.ToCollection();


        _memoryTabMatrix = new (3, _catalogGridWidth, "MemoryCatalog");
        _memoryTabMatrix.CollectionIntegrate( new List<ControlItem>()
        {
            new ControlItem() {Sector = "Memory", Name="MemoryClear", Text = "MC", Position = new (){{"x", 0}, {"y", 0}}},
            new ControlItem() {Sector = "Memory", Name="MemoryRead", Text = "MR", Position = new (){{"x", 1}, {"y", 0}}},
            new ControlItem() {Sector = "Memory", Name="MemorySave", Text = "MS", Position = new (){{"x", 2}, {"y", 0}}},
            new ControlItem() {Sector = "Memory", Name="MemoryView", Text = "M↓", Position = new (){{"x", 0}, {"y", 1}}},
            new ControlItem() {Sector = "Memory", Name="ChangeView", Text = "F-E", Position = new (){{"x", 1}, {"y", 1}}},
            new ControlItem() {Sector = "Memory", Name="MemoryNumberAddNumber", Text = "M+", Position = new (){{"x", 2}, {"y", 1}}},
            new ControlItem() {Sector = "Memory", Name="MemoryNumberSubtractNumber", Text = "M-", Position = new (){{"x", 0}, {"y", 2}}},
        });
        _memoryTabMatrixItems = _memoryTabMatrix.ToCollection();


        // _customFunctionTabMatrix = new (0, _catalogGridWidth, "CustomFunctionCatalog");
        // _customFunctionTabMatrix.CollectionIntegrate( new List<ControlItem>()
        // {
        // });
        // _customFunctionTabMatrixItems = _customFunctionTabMatrix.ToCollection();


        // Dictionary<string, int> variableDict = JsonStorage.Configurations[Config.CurrentConfig]["Variables"].ToObject<Dictionary<string, int>>();
        // _variableTabMatrix = new ((ushort)(variableDict.Count / _catalogGridWidth), _catalogGridWidth, "VariableCatalog");

        // List<ControlItem> variableItems = new List<ControlItem>();
        // uint counter = 0;
        // foreach(KeyValuePair<string, int> pair in variableDict)
        // {
        //     variableItems.Add(new () {Sector="Variables", Name=pair.Key, Text=pair.Key, Position= new (){{"x", (int)counter % _catalogGridWidth}, {"y", (int)counter / _catalogGridWidth}}});
        //     counter++;
        // }

        // _variableTabMatrix.CollectionIntegrate(variableItems);
        // _variableTabMatrixItems = _variableTabMatrix.ToCollection();

        InputController.Update += DisplayUpdate;
        SetJsonData();
    }

    private void SetJsonData()
    {
        JObject config = JsonStorage.Configurations[Global.currentConfigName].ToObject<JObject>();

        List<ushort> buttonsSize=  config["TableStructs"]["Controls"].ToObject<List<ushort>>();
        _buttonsMatrix = new (buttonsSize[0], buttonsSize[1], "ButtonControls");
        _buttonMatrixWidth = buttonsSize[1];
        _buttonMatrixHeight = buttonsSize[0];
        List<ushort> displaySize=  config["TableStructs"]["Display"].ToObject<List<ushort>>();
        _displayMatrix = new (displaySize[0], displaySize[1], "DisplayControls");

        List<ControlItem> buttonControls = new List<ControlItem>();
        List<ControlItem> displayControls = new List<ControlItem>();
        foreach (JObject jobject in config["ControlsLayout"].ToObject<List<JObject>>())
        {
            List<int> buttonPosotion=  jobject["Position"].ToObject<List<int>>();
            ControlItem item  = new ControlItem() {
                Sector = jobject["Sector"].Value<string>(), 
                Name=jobject["Name"].Value<string>(),  
                StorageType=jobject["StorageType"].Value<string>(), 
                Position = new (){{"x", buttonPosotion[0]}, {"y", buttonPosotion[1]}}
                };

            if (jobject["Sector"].Value<string>() == "Display")
            {
                displayControls.Add(item);
            }
            else
            {
                item.Text = jobject["Text"].Value<string>();
                buttonControls.Add(item);  
            }
        }
        _buttonsMatrix.CollectionIntegrate(buttonControls);
        _displayMatrix.CollectionIntegrate(displayControls);
        
        _buttonMatrixItems = _buttonsMatrix.ToCollection();
        _displayMatrixItems = _displayMatrix.ToCollection();
    }

    #region ToolsArea

    [ObservableProperty]
    private bool _controlsIsChecked;

    [ObservableProperty]
    private bool _layoutsIsChecked;

    [RelayCommand]
    public void TogglePanelFC()
    {
        IsPanelFCOpen = !IsPanelFCOpen;
        PanelFCHeight = IsPanelFCOpen ? FCHeightMax : 0;
    }

    [RelayCommand]
    public void TestCommand()
    {
        // GlobWidth += 1;
    }



    [RelayCommand]
    public void TriggerPaneLP()
    {
        IsPaneLPOpen = !IsPaneLPOpen;
    }

    #endregion

    #region CalculatorArea
    private ControlMatrix<ControlItem> _displayMatrix;

    [ObservableProperty]
    private int _displayMatrixHeight;

    [ObservableProperty]
    private ObservableCollection<Panel> _displayMatrixItems;

    [ObservableProperty]
    private ControlMatrix<ControlItem> _buttonsMatrix;

    [ObservableProperty]
    private int _buttonMatrixWidth;

    [ObservableProperty]
    private int _buttonMatrixHeight;
    

    [ObservableProperty]
    private ObservableCollection<Panel> _buttonMatrixItems;

    #endregion

    #region CatalogArea

    [ObservableProperty]
    private bool _isPanelFCOpen;

    [ObservableProperty]
    private double _panelFCHeight;

    [ObservableProperty]
    private string? _selectedListItem;

    [ObservableProperty]
    private ushort _catalogGridWidth;

    private void DisplayUpdate()
    {
        string newText = Output.ExpressionToRecord(Global.expression);
        foreach (Panel panel in _displayMatrixItems)
        {
            if (panel.Children.Count > 0 && panel.Children[0] is TextBox box)
            {
                box.Text = newText;
            }
        }
    }

    // DisplayTab
    [ObservableProperty]
    private ControlMatrix<ControlItem> _displayTabMatrix;

    [ObservableProperty]
    private ObservableCollection<Panel> _displayTabMatrixItems;

    // NumbersTab
    [ObservableProperty]
    private ControlMatrix<ControlItem> _numbersTabMatrix;

    [ObservableProperty]
    private ObservableCollection<Panel> _numbersTabMatrixItems;

    // ActionsTab
    [ObservableProperty]
    private ControlMatrix<ControlItem> _actionsTabMatrix;

    [ObservableProperty]
    private ObservableCollection<Panel> _actionsTabMatrixItems;

    // OperatorsTab
    [ObservableProperty]
    private ControlMatrix<ControlItem> _operatorsTabMatrix;

    [ObservableProperty]
    private ObservableCollection<Panel> _operatorsTabMatrixItems;

    // FunctionTab
    [ObservableProperty]
    private ControlMatrix<ControlItem> _functionTabMatrix;

    [ObservableProperty]
    private ObservableCollection<Panel> _functionTabMatrixItems;

    // TrigonometryTab
    [ObservableProperty]
    private ControlMatrix<ControlItem> _trigonometryTabMatrix;

    [ObservableProperty]
    private ObservableCollection<Panel> _trigonometryTabMatrixItems;
    
    // MemoryTab
    [ObservableProperty]
    private ControlMatrix<ControlItem> _memoryTabMatrix;

    [ObservableProperty]
    private ObservableCollection<Panel> _memoryTabMatrixItems;

    // CustomFunctionTab
    [ObservableProperty]
    private ControlMatrix<ControlItem> _customFunctionTabMatrix;

    [ObservableProperty]
    private ObservableCollection<Panel> _customFunctionTabMatrixItems;

    // VariableTab

    [ObservableProperty]
    private ControlMatrix<ControlItem> _variableTabMatrix;

    [ObservableProperty]
    private ObservableCollection<Panel> _variableTabMatrixItems;


    #endregion

    #region LayoutsArea

    [ObservableProperty]
    private bool _isPaneLPOpen;

    [ObservableProperty]
    private ObservableCollection<string> _layoutItems;

    #endregion

}

public class ControlMatrix<T>
{
    public ControlMatrix(ushort height, ushort width, string storageType)
    {
        Height = height;
        Width = width;

        StorageType = storageType;

        Matrix = new();

        for (int i = 0; i < Height; i++)
        {
            Matrix.Add(new ());

            for (int j = 0; j < Width; j++)
            {
                Matrix[i].Add(NewPanel(new(){StorageType = StorageType, Position = new (){{"x", i}, {"y", j}}}, Matrix));
            }
        }
    }

    public ObservableCollection<Panel> Observer {get;set;}
    public string StorageType {get;}

    public void SetObserver()
    {
        Observer = ToCollection();
    }

    public ControlMatrix(List<List<Panel>> matrix)
    {
        Matrix = matrix;

        Height = (ushort)matrix.Count;
        Width = (ushort)matrix.First().Count;
    }

    public ObservableCollection<Panel> ToCollection()
    {
        ObservableCollection<Panel> collection = new();

        for (uint i = 0; i < Height; i++)
        {
            for (uint j = 0; j < Width; j++)
            {
                collection.Add(Matrix[(int)i][(int)j]);
            }
        }
        
        return collection;
    }

    public void CollectionIntegrate(IEnumerable<ControlItem> collection)
    {
        foreach (ControlItem item in collection) 
        {
            Control control;
            switch (StorageType)
            {
                case "DisplayCatalog" or "DisplayControls":
                {
                    control = new TextBox();
                    break; 
                } 
                default:
                {
                    ControlItem buttonItem = item as ControlItem;
                    buttonItem.StorageType = StorageType;
                    Button button;
                    if (StorageType.Contains("Catalog"))
                    {
                        button = NewDragButton(buttonItem);
                    }
                    else
                    {
                        button = NewButton(buttonItem);
                        button.AddHandler(Button.ClickEvent, InputController.GetActionByButtonData(buttonItem.Data));
                    }
                    control = button;
                    break;
                }
            };
            Dictionary<string, int> position = item.Position;
            if (position["y"] < Matrix.Count && position["x"] < Matrix.First().Count)
                Matrix[position["y"]][position["x"]].Children.Add(control);
        }
    }

    public List<List<Panel>> Matrix {get;}
    
    public ushort Width {get;}
    public void UpWidth()
    {
        for(int i = 0, j = Matrix.First().Count; i < Height; i++)
        {
            Matrix[i].Add(NewPanel(new (){StorageType = StorageType, Position = new (){{"x", i}, {"y", j}}}, Matrix));
        }
    } 
    public void DownWidth()
    {
        for(int i = 0, l = Matrix[i].Count - 1; i < Height; i++)
        {
            Matrix[i].RemoveAt(l);
        }
    }
    public ushort Height {get;}
    public void UpHeight()
    {
        Matrix.Add(new());
        for(int i = 0, l = Matrix.Count - 1; i < Width; i++)
        {
            Matrix[l].Add(NewPanel(new ControlItem() {StorageType = StorageType, Position = new (){{"x", l}, {"y", i}}}, Matrix));
        }
    } 
    public void DownHeight()
    {
        Matrix.RemoveAt(Matrix.Count - 1);
    }

    public static Panel NewPanel(ControlItem item, List<List<Panel>> Matrix)
    {
        Panel panel = new PanelPlus() {StorageType=item.StorageType, Position=item.PositionList} as Panel;
        panel.Background = Brush.Parse("#1e1e1e");
    
        DragDrop.SetAllowDrop(panel, true);
        panel.AddHandler(DragDrop.DragOverEvent, (object sender, DragEventArgs e) => {
            JObject content = e.Data.Get("ButtonContent") as JObject;
            string oldStrageType = content["StorageType"].Value<string>();
            string newStorageType = (sender as PanelPlus).StorageType;
            if (
                e.Data.Contains("ButtonContent") & 
                !(sender as PanelPlus).StorageType.Contains("Catalog") &
                (((sender as Panel).Children.Count == 0 & ((oldStrageType == "DisplayCatalog" & newStorageType == "DisplayControls") | (oldStrageType != "DisplayCatalog"& newStorageType != "DisplayControls"))) | 
                ((sender as Panel).Children.Count != 0 & (content["Name"].Value<string>() == "DeleteElement"))))
            {
                e.DragEffects = DragDropEffects.Copy;
            }
            else
            {
                e.DragEffects = DragDropEffects.None;
            }
        });
        panel.AddHandler(DragDrop.DropEvent, (object sender, DragEventArgs e) => {
            JObject content = e.Data.Get("ButtonContent") as JObject;
            Button button = NewButton(new ControlItem() {StorageType = (sender as PanelPlus).StorageType, Text = content["Text"].Value<string>()});
            button.AddHandler(Button.ClickEvent, InputController.GetActionByButtonData(content));
            Panel panel = sender as Panel;
            for (int i = 0; i < Matrix.Count; i++)
            {
                for (int j = 0; j < Matrix[i].Count; j++)
                {
                    if(Matrix[i][j] == panel)
                    {
                        JObject jobject = new JObject
                        {
                            {"Sector", content["Sector"].Value<string>()},
                            {"Name", content["Name"].Value<string>()},
                            {"StorageType", (sender as PanelPlus).StorageType},
                            {"Text", content["Text"].Value<string>()},
                            {"Position", new JArray(new List<int>() {j, i})}
                        }; 
                        if (content["Name"].Value<string>() == "DeleteElement") 
                        {
                            if (Matrix[i][j].Children.Count != 0) Matrix[i][j].Children.Clear();
                            int counter = 0;
                            foreach(JObject ls in JsonStorage.Configurations[Global.currentConfigName]["ControlsLayout"].ToObject<List<Object>>())
                            {
                                List<int> ps = ls["Position"].ToObject<List<int>>();
                                if (j == ps[0] && i == ps[1]) 
                                {
                                    (JsonStorage.Configurations[Global.currentConfigName]["ControlsLayout"] as JArray).RemoveAt(counter);
                                    break;
                                }
                                counter++;
                            }
                        }
                        else 
                        {
                            Matrix[i][j].Children.Add(button);
                            (JsonStorage.Configurations[Global.currentConfigName]["ControlsLayout"] as JArray).Add(jobject);
                        }
                        JsonStreamer.WriteConfigurations(JsonStorage.Configurations);
                        return;
                    }
                }
            }
        });
        return panel;
    }

    public static Button NewButton(ControlItem item)
    {
        Button button = new Button() {
            Content = item.Text,
            HorizontalAlignment=HorizontalAlignment.Stretch,
            VerticalAlignment=VerticalAlignment.Stretch,
            HorizontalContentAlignment=HorizontalAlignment.Center,
            VerticalContentAlignment=VerticalAlignment.Center
        };
        return button;
    }

    public static Button NewDragButton(ControlItem item)
    {
        Button button = NewButton(item);
        button.AddHandler(
            InputElement.PointerReleasedEvent, 
            (object sender, PointerPressedEventArgs e)=> 
            {
                
            }, handledEventsToo: true);
        button.AddHandler(
            InputElement.PointerPressedEvent, 
            (object sender, PointerPressedEventArgs e) =>
            {
                var button = sender as Button;
                var data = new DataObject();
                data.Set("ButtonContent", item.Data); // Передаем данные

                // Начинаем перетаскивание
                DragDrop.DoDragDrop(e, data, DragDropEffects.Copy);
            }, 
            handledEventsToo: true
        );
        return button;
    }

}

