using Avalonia.Data.Converters;

public class BoolToBoolForCatalog : FuncValueConverter<bool, bool>
{
    public BoolToBoolForCatalog() : base(isChecked => !isChecked) {}
}