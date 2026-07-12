using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using BridgeSystems.Bridgemate.DataConnector.ScoringProgramClient;

namespace BridgeSystems.Bridgemate.DataConnectorClientEmulator.Converters;

public class ErrorTypeToColorConverter : IValueConverter
{
    public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var errorType = (ErrorType)value;

        var result = errorType == ErrorType.None ? Colors.Black : Colors.Red;
        return result;
    }

    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}
