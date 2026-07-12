using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BridgeSystems.Bridgemate.DataConnectorClientEmulator;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        icComResults.ItemContainerGenerator.StatusChanged += (s, e) =>
        {
            if (icComResults.ItemContainerGenerator.Status ==
                GeneratorStatus.ContainersGenerated)
            {
               svComResults.ScrollToEnd();
            }
        };

        icComResultsAdvanced.ItemContainerGenerator.StatusChanged += (s, e) =>
        {
            if (icComResultsAdvanced.ItemContainerGenerator.Status ==
                GeneratorStatus.ContainersGenerated)
            {
                svComResultsAdvanced.ScrollToEnd();
            }
        };

        icComResultsManual.ItemContainerGenerator.StatusChanged += (s, e) =>
        {
            if (icComResultsManual.ItemContainerGenerator.Status ==
                GeneratorStatus.ContainersGenerated)
            {
                svComResultsManual.ScrollToEnd();
            }
        };

    }

    private void hlCreate_Click(object sender, RoutedEventArgs e)
    {
        scCreate.BringIntoView();
    }

    private void hlModify_Click(object sender, RoutedEventArgs e)
    {
        scModify.BringIntoView();
    }

    private void hlSendAndPoll_Click(object sender, RoutedEventArgs e)
    {
        scSendAndPoll.BringIntoView();
    }

    private void HyperlinkBack_Click(object sender, RoutedEventArgs e)
    {
        scTop.BringIntoView();
    }
}