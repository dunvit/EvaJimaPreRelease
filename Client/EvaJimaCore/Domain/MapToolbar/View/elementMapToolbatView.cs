using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using EveJimaCore.Domain.MapToolbar.Contracts;
using EveJimaCore.WhlControls;

namespace EveJimaCore.Domain.MapToolbar.View
{
    public partial class ElementMapToolbatView : Form, IMapToolbarView
    {
        readonly Dictionary<string, ejButton> _toolbarControls = new Dictionary<string, ejButton>();

        public string SelectedTab { get; set; }

        public event Action<string> SelectTab;

        public ElementMapToolbatView()
        {
            InitializeComponent();
        }

        private void ElementMapToolbatView_Load(object sender, EventArgs e)
        {

            AddToolbarElement("SolarSystem", cmdSystemInformation);
            AddToolbarElement("MapSignatures", cmdMapSignatures);
            AddToolbarElement("FleetInformation", cmdFleetInformation);
            AddToolbarElement("MapSettings", cmdMapSettings);

            // Activate default tab
            ActivatePanel("SolarSystem");
        }

        private void AddToolbarElement(string elementName, ejButton button)
        {
            button.Tag = elementName;

            _toolbarControls.Add(elementName, button); 
        }

        private void ActivatePanel(string panelName)
        {
            foreach(var control in _toolbarControls.Values)
            {
                control.ForeColor = Color.Silver;
            }

            _toolbarControls[panelName].ForeColor = Color.DarkGoldenrod;

            if (SelectTab != null) SelectTab(panelName);

            SelectedTab = panelName;
        }

        private void Event_SelectElement(object sender, MouseEventArgs e)
        {
            var element = ((Control)sender).Tag as string;

            ActivatePanel(element);
        }

        private void cmdMapSettings_Click(object sender, EventArgs e)
        {

        }
    }
}
