using System;
using System.Windows.Forms;
using EvaJimaCore;
using log4net;

namespace EveJimaCore.Logic
{
    public partial class ScreenUpdateToServer : Form
    {
        readonly ILog _commandsLog = LogManager.GetLogger("CommandsMap");

        public ScreenUpdateToServer()
        {
            InitializeComponent();

            Activated += Event_Activate;

            Global.Pilots.Selected.SpaceMap.OnChangeStatus += Event_ChangeStatus;
        }

        private void Event_ChangeStatus(string message)
        {
            _commandsLog.InfoFormat("[ScreenUpdateToServer.Event_ChangeStatus] " + message);

            UpdateActionLog(message);
        }

        private void Event_Activate(object sender, EventArgs e)
        {
            Event_ChangeStatus($"Start delete solar system '{Global.Pilots.Selected.SpaceMap.SelectedSolarSystemName}' for map with key '{Global.Pilots.Selected.SpaceMap.Key}'.");

            Global.MapApiFunctions.DeleteSolarSystem(Global.Pilots.Selected.SpaceMap.Key, Global.Pilots.Selected.SpaceMap.SelectedSolarSystemName, Global.Pilots.Selected.Name);

            Global.Pilots.Selected.SpaceMap.RemoveSystem(Global.Pilots.Selected.SpaceMap.SelectedSolarSystemName);

            Close();
        }

        public void UpdateActionLog(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateActionLog(message)));
                return;
            }

            lblUpdateLog.Text = "";
        }

        private void ScreenUpdateToServer_FormClosing(object sender, FormClosingEventArgs e)
        {
            Global.Pilots.Selected.SpaceMap.OnChangeStatus -= Event_ChangeStatus;
        }
    }
}
