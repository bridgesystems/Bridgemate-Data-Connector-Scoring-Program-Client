using System.Globalization;
using System.Windows.Data;
using BridgeSystems.Bridgemate.DataConnectorClientEmulator.Support;

namespace BridgeSystems.Bridgemate.DataConnectorClientEmulator.Converters;

public class ScoringTypeConverter : IValueConverter
{
    private static readonly List<ScoringTypeSelection> ScoringTypes = ScoringTypeSelection.GetScoringTypesList();
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return ScoringTypes.Single(st => st.ScoringType == (int)value).Description;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => 
        Binding.DoNothing;
}
