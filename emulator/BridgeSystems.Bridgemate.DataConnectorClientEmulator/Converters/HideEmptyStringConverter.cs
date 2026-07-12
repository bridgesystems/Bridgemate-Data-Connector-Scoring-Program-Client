using System.Globalization;

namespace BridgeSystems.Bridgemate.DataConnectorClientEmulator.Converters;

public class HideEmptyStringConverter : BoolToVisibilityConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var text = (string)value;
        var isNotNullOrEmpty = !string.IsNullOrEmpty(text);
        return base.Convert(isNotNullOrEmpty, targetType, parameter, culture);
    }
}
