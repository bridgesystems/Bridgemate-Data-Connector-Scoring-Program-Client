using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BridgeSystems.Bridgemate.DataConnectorClientEmulator.Viewmodels;
partial class ObservableEvent:ObservableObject
{
    public static string? Guid
    {
        get;set;
    }
    public ObservableEvent(ObservableSession firstSession)
    {
        Sessions = new ObservableCollection<ObservableSession>([firstSession]);
        CurrentSession = Sessions.First();
    }

    public string? EventGuid
    {
        get => Guid;
        set
        {
            Guid = value;
            OnPropertyChanged(nameof(EventGuid));
        }
    }

    [ObservableProperty]
    ObservableCollection<ObservableSession> sessions;
    [ObservableProperty]
    ObservableSession currentSession;
}
