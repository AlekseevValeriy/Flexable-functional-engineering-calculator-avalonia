using System.Collections.Generic;
using Avalonia.Controls;

namespace FFECAvalonia.CustomControls;
public class PanelPlus: Panel
{
    public required string StorageType {get; init;}
    public required List<int> Position {get; init;}
    public PanelPlus() : base() {}
}