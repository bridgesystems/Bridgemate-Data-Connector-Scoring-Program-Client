using System.Globalization;

namespace BridgeSystems.Bridgemate.DataConnectorClientEmulator.Converters;

public class NumberToVisibilityConverter : BoolToVisibilityConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var number = value as int?;
        var stringParameter = (string)parameter;
        var equalOrGreaterThan = false;
        if (stringParameter.EndsWith("+"))
        {
            equalOrGreaterThan = true;
            stringParameter = stringParameter.Substring(0, stringParameter.Length - 1);
        }
        var parameterPresent = int.TryParse(stringParameter, out var compare);
        var equals = number.HasValue && parameterPresent && (equalOrGreaterThan ? number >= compare : number == compare);
        return base.Convert(equals, targetType, "collapse", culture);
    }
}