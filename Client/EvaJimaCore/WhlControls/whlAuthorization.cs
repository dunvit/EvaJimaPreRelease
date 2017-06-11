using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using EvaJimaCore;
using EveJimaCore.BLL;
using EveJimaCore.BLL.Map;
using EveJimaCore.Domain.Map.Contracts;
using EveJimaCore.Domain.Map.Modes;
using EveJimaCore.Domain.Map.Presenter;
using EveJimaCore.Domain.Map.View;
using log4net;

namespace EveJimaCore.WhlControls
{
    public partial class whlAuthorization : baseContainer
    {

        public DelegateChangeSelectedPilot OnChangeSelectedPilot { get; set; }

        private static readonly ILog Log = LogManager.GetLogger(typeof(whlAuthorization));

        private const string TextAuthorizationInfo =
            "To login you will need to press the button and go to the  CCP SSO (single sign-on) site. All your private data will remain on the CCP's website.";

        private const string TextAfterAuthorizationInfo =
            "You have successfully logged into the system and the EveJima can now keep track of your current position. You can log in again with another character.";

        private const string TextErrorAuthorizationInfo = "It has failed to create a local server. Log in CCP SSO (single sign-on) site is not possible.";

        private const string TextPleaseWaitLoadingPilots = "Loading data from the pilots cache. Please wait. It may take a few seconds.";

        public whlAuthorization()
        {
            InitializeComponent();

            Task.Run(() =>
            {
                try
                {
                    LoadAllPilotesFromStorage();
                }
                catch (Exception ex)
                {
                    Log.ErrorFormat("[whlAuthorization.whlAuthorization] Critical error. Exception {0}", ex);
                }
                
            });
        }

        List<PilotEntity> Pilotes { get; set; }

        private bool _isLoadedPilotesFromStorage;

        public void LoadAllPilotesFromStorage()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => LoadAllPilotesFromStorage()));
            }

            Pilotes = new List<PilotEntity>();

            foreach(var pilot in Global.ApplicationSettings.Pilots)
            {
                try
                {
                    var currentPilot = new PilotEntity(pilot.Item2, pilot.Item3);

                    currentPilot.Key = pilot.Item4 != "" ? pilot.Item4 : currentPilot.Name.Replace(" ", "_").Replace("-", "_");

                    Pilotes.Add(currentPilot);
                }
                catch (Exception ex)
                {
                    Log.ErrorFormat("[whlAuthorization.LoadAllPilotesFromStorage] Critical error. Exception {0}", ex);
                }
                
            }

            ShowPilots();

            _isLoadedPilotesFromStorage = true; 
        }

        private void Event_GoToCCPSSO(object sender, EventArgs e)
        {
            var data = WebUtility.UrlEncode(@"http://localhost:" + Global.Settings.CCPSSO_AUTH_PORT + "/WormholeLocator");

            var address = "https://login.eveonline.com/oauth/authorize?response_type=code&redirect_uri=" + data +
                          "&client_id=" + Global.Settings.CCPSSO_AUTH_CLIENT_ID + "&scope=" + Global.Settings.CCPSSO_AUTH_SCOPES + "&state=" +
                          Global.Settings.CCPSSO_AUTH_CLIENT_STATE + "";


            #region ESI
            //var address = "https://login.eveonline.com/oauth/authorize?response_type=code&redirect_uri=" + data +
            //              "&client_id=" + Global.Settings.CCPSSO_AUTH_CLIENT_ID +
            //              "&scope=" + Global.Settings.CCPSSO_AUTH_SCOPES + "&state=" +
            //              Global.Settings.CCPSSO_AUTH_CLIENT_STATE + "";
            #endregion

            Process.Start(address);

        }

        public void PilotAuthorizeFlow(string code)
        {
            Log.DebugFormat("[whlAuthorization.PilotAuthorizeFlow] starting for token = {0}", code);

            var _currentPilot = new PilotEntity(code);

            //_currentPilot.Initialization(code);

            Global.Metrics.PublishOnPilotInitialization(_currentPilot.Id);

            if (Global.Pilots.IsExist(_currentPilot.Id) == false)
            {

                Global.ApplicationSettings.UpdatePilotInStorage(_currentPilot.Name, _currentPilot.Id.ToString(), _currentPilot.CrestData.RefreshToken);

                Global.Pilots.Add(_currentPilot);

                cmbPilots.Items.Add(_currentPilot.Name.Trim());
                cmbPilots.Text = _currentPilot.Name.Trim();
            }
            else
            {
                // Update token
                Global.ApplicationSettings.UpdatePilotInStorage(_currentPilot.Name, _currentPilot.Id.ToString(), _currentPilot.CrestData.RefreshToken);
            }

            cmbPilots.Visible = true;

            Global.Pilots.Selected = _currentPilot;
        }


        public void SelectPilot(string pilot)
        {
            cmbPilots.SelectedIndex = cmbPilots.FindString(pilot);
            Log.DebugFormat("[whlAuthorization.SelectPilot] cmbPilots.SelectedIndex {0}", cmbPilots.FindString(pilot).ToString());
            RefreshPilotInfo();
        }

        public void RefreshPilotInfo()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => RefreshPilotInfo()));
            }

            crlPilotPortrait.Image = Global.Pilots.Selected.Portrait;
            crlPilotPortrait.Refresh();

            crlPilotPortrait.Visible = true;

            lblAuthorizationInfo.Text = TextAfterAuthorizationInfo + Environment.NewLine + Environment.NewLine + TextAuthorizationInfo;

            if (OnChangeSelectedPilot != null) OnChangeSelectedPilot();
        }


        private void ShowPilots()
        {
            if (Pilotes.Count <= 0) return;

            foreach (var pilotEntity in Pilotes)
            {
                Global.Pilots.Add(pilotEntity);

                cmbPilots.Items.Add(pilotEntity.Name.Trim());
                cmbPilots.Text = pilotEntity.Name.Trim();

                Global.Pilots.Selected = pilotEntity;
            }

            if (cmbPilots.InvokeRequired)
            {
                cmbPilots.Invoke(new MethodInvoker(delegate
                {
                    btnLogInWithEveOnline.Visible = true;
                    cmbPilots.Visible = true;
                    crlPilotPortrait.Image = Global.Pilots.Selected.Portrait;
                    crlPilotPortrait.Refresh();
                    crlPilotPortrait.Visible = true;
                    if (OnChangeSelectedPilot != null) OnChangeSelectedPilot();
                    lblAuthorizationInfo.Text = TextAfterAuthorizationInfo + Environment.NewLine + Environment.NewLine + TextAuthorizationInfo;
                }));
            }

            if ( Global.Pilots.Selected != null )
            {
                Global.Pilots.Activate(Global.Pilots.Selected.Name);
            }
        }


        private void cmbPilots_TextChanged(object sender, EventArgs e)
        {
            if (_isLoadedPilotesFromStorage)
            {

                Global.Pilots.Activate(cmbPilots.Text);

                RefreshPilotInfo();
            }
        }

        public void RefreshAuthorizationStatus()
        {
            if (Global.Settings.IsAuthorizationEnabled)
            {
                lblAuthorizationInfo.Text = TextAuthorizationInfo;
            }
            else
            {
                lblAuthorizationInfo.Text = TextErrorAuthorizationInfo;
                btnLogInWithEveOnline.Visible = false;
            }

            //string[] allLines = Global.Pilots.GetPilotsStorageContent();

            //if (allLines.Count() > 0)
            //{
            //    lblAuthorizationInfo.Text = TextPleaseWaitLoadingPilots + Environment.NewLine + Environment.NewLine;
            //    btnLogInWithEveOnline.Visible = false;
            //}

            
        }

        private void cmbPilots_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
