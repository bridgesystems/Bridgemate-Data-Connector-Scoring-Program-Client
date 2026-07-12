using System.Globalization;
using System.Windows.Data;
using BridgeSystems.Bridgemate.DataConnectorClientEmulator.Support;

namespace BridgeSystems.Bridgemate.DataConnectorClientEmulator.Converters;

public class GameTypeConverter : IValueConverter
{
    private static readonly List<GameTypeSelection> GameTypes = GameTypeSelection.GetGameTypesList();
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return GameTypes.Single(st => st.GameType == (int)value).Description;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        Binding.DoNothing;
}
