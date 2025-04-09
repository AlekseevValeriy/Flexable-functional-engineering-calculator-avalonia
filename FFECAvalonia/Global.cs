global using System;
global using System.Collections.Generic;
global using System.Drawing;
global using System.Linq;
global using System.Text;
global using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using FFEC;
using FFEC.ExpressionModule.Expression;
using FFECAvalonia;

public static class Global
{
    internal static List<Composite> expression = [];
    internal static string currentConfigName = Config.CurrentConfig; 
    internal static IDialogService dialogService;

    public static async Task<string?> ShowDialogAsync()
    {
        string? result = await dialogService.ShowTextInputDialogAsync();

        return !string.IsNullOrEmpty(result) ? result : null;

    }
}
