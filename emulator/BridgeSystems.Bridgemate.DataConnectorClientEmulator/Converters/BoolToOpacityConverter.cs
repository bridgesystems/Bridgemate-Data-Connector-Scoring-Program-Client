using System.Globalization;
using System.Windows.Data;

namespace BridgeSystems.Bridgemate.DataConnectorClientEmulator.Converters;

public class BoolToOpacityConverter : IValueConverter
{
    public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var param = System.Convert.ToString(parameter) ?? "none";
        param = param.ToLower();
        var full = System.Convert.ToBoolean(value);
        if (param.EndsWith("inverted"))
            full = !full;
        var result = full ? 1.0:0.4;
        return result;
    }

    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}
