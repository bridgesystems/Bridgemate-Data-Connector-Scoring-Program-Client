using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BridgeSystems.Bridgemate.DataConnectorClientEmulator.Converters;

public class BoolToVisibilityConverter : IValueConverter
{
    public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var param = System.Convert.ToString(parameter) ?? "none";
        param = param.ToLower();
        var show = System.Convert.ToBoolean(value);
        if (param.EndsWith("inverted"))
            show = !show;
        var result = show ? Visibility.Visible : param.StartsWith("collapse") ? Visibility.Collapsed : Visibility.Hidden;
        return result;
    }

    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}

public class NotConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            return !(bool)value;
        }
        catch
        {
            return Binding.DoNothing;
        }
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}
