using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CsvHelper;
using EvaJimaCore;
using EveJimaCore.BLL;
using EveJimaCore.BLL.Map;
using EveJimaCore.UiTools;
using EveJimaUniverse;
using log4net;

namespace EveJimaCore.WhlControls
{

    public partial class whlMap : BaseContainer
    {
        private static readonly ILog Log = LogManager.GetLogger("All");
        readonly ILog _commandsLog = LogManager.GetLogger("Commands");

        int screenSizeWidth = 600;
        int screenSizeHeight = 500;

        List<string> _systems = new List<string>();
        List<SystemLine> _lines = new List<SystemLine>();

        private Timer _mapReloader = new Timer();

        public Map SpaceMap { get; set; }

        public string LocationSolarSystemName { get; set; }

        public string MapKey { get; set; }

        public string SelectedSolarSystemName { get; set; }

        public string PreviousLocationSolarSystemName { get; set; }

        public string Pilot { get; set; }

        public whlMap()
        {
            InitializeComponent();
        }

        private void whlMap_Load(object sender, EventArgs e)
        {
            _mapReloader = new Timer { Interval = 10000 };

            _mapReloader.Tick += UpdateSpaceMap;

            screenSizeWidth = containerMap.Width;
            screenSizeHeight = containerMap.Height;
        }

        public override void ActivateContainer()
        {
            _mapReloader.Enabled = true;

            //var a = CrestApiFunctions.GetLocation(Global.Pilots.Selected.Id, Global.Pilots.Selected.CrestData.AccessToken);

            if (Global.Pilots.Selected == null) return;

            MapKey = Global.Pilots.Selected.Key;

            LocationSolarSystemName = Global.Pilots.Selected.Location.SolarSystemName;

            SelectedSolarSystemName = LocationSolarSystemName;

            SpaceMap = Global.Pilots.Selected.SpaceMap;

            Pilot = Global.Pilots.Selected.Name;
            

            MapTools.Normalization(SpaceMap);

            DrawSelectedSolarSystemInformation(LocationSolarSystemName);

            //ShowSignaturesForSystem(LocationSolarSystemName);

            var currentSystem = SpaceMap.GetSystem(LocationSolarSystemName);

            if (currentSystem == null) return;

            UpdateSpaceMap();

            DrawMap();
        }

        private void UpdateSpaceMap()
        {
            if (SpaceMap != null)
            {
                var message = SpaceMap.Update();

                _commandsLog.InfoFormat(message);

                if (SpaceMap.Pilotes == null) return;

                foreach (var pilot in SpaceMap.Pilotes)
                {
                    var controlName = pilot.Name.Replace(" ", "_");

                    var _solarSystem = SpaceMap.GetSystem(pilot.System);

                    if (_solarSystem == null) return;

                    if (containerMap.Controls[controlName] == null)
                    {
                        var positionPilots = new Rectangle(_solarSystem.LocationInMap.X - mapPositionX - 5, 
                                    _solarSystem.LocationInMap.Y - mapPositionY - 5, 10, 10);

                        var pilotesInSystem = new Panel
                        {
                            Name = controlName,
                            BackColor = Color.DarkMagenta,
                            Size = new Size(10, 10),
                            Location = new Point(positionPilots.X, positionPilots.Y),
                            Visible = true,
                            Tag = pilot.System
                        };

                        pilotesInSystem.Click += Event_SelectSolarSystem;

                        var toolTip1 = new ToolTip();

                        var pilotes = SpaceMap.GetPilotesBySolarSystem(_solarSystem.Name);

                        toolTip1.SetToolTip(pilotesInSystem, pilotes);

                        containerMap.Controls.Add(pilotesInSystem);
                    }
                    else
                    {
                        var positionPilots = new Rectangle(_solarSystem.LocationInMap.X - mapPositionX - 5, 
                                    _solarSystem.LocationInMap.Y - mapPositionY - 5, 10, 10);

                        var panel = (Panel)containerMap.Controls[controlName];

                        panel.Location = new Point(positionPilots.X, positionPilots.Y);
                    }


                }

                MapTools.Normalization(SpaceMap);
            }

            containerMap.Refresh();
        }

        private void Event_SelectSolarSystem(object sender, EventArgs e)
        {
            var solarSystemName = sender as Panel;

            ReDrawSolarSystemInformationPanel(solarSystemName.Tag.ToString());
        }

        private void UpdateSpaceMap(object sender, EventArgs e)
        {
            UpdateSpaceMap();
        }

        private void DrawSelectedSolarSystemInformation(string systemName)
        {
            if (systemName == "unknown" || systemName == null) return;

            containerSolarSystemInfo.Controls.Clear();

            SelectedSolarSystemName = systemName;

            var system = Global.Space.GetSolarSystem(systemName);

            if (system.Security == SecurityStatus.WSpace)
            {
                var solarSystemInformation = new mapSolarSystemWSpace();

                solarSystemInformation.RefreshSolarSystem(system);

                solarSystemInformation.Location = new Point(1, 1);
                solarSystemInformation.Visible = true;

                containerSolarSystemInfo.Controls.Add(solarSystemInformation);
            }
            else
            {
                var solarSystemInformation = new mapSolarSystemKSpace();

                solarSystemInformation.RefreshSolarSystem(system);

                solarSystemInformation.Location = new Point(1, 1);
                solarSystemInformation.Visible = true;

                containerSolarSystemInfo.Controls.Add(solarSystemInformation);
            }
        }


        private int mapPositionX = 0;
        private int mapPositionY = 0;

        private void DrawMap()
        {
            if (LocationSolarSystemName == null) return;

            //LocationSolarSystemName = Global.Pilots.Selected.Location.SolarSystemName;

            _systems = new List<string>();

            _lines = new List<SystemLine>();

            //containerMap.Controls.Clear();

            var currentSystem = SpaceMap.GetSystem(LocationSolarSystemName);

            if (currentSystem == null) return;

            DrawSolarSystem(currentSystem);

            //containerMap.Refresh();
        }

        private void DrawSolarSystem(SolarSystem solarSystem)
        {
            _systems.Add(solarSystem.Name);

            foreach (var connectedSolarSystemName in solarSystem.Connections)
            {
                var connectedSolarSystem = SpaceMap.GetSystem(connectedSolarSystemName);

                if (connectedSolarSystem != null)
                {
                    if (_systems.Contains(connectedSolarSystem.Name) == false)
                    {
                        DrawSolarSystem(connectedSolarSystem);
                    }

                    var connectionType = MapTools.GetConnectionType(connectedSolarSystem, solarSystem);

                    var line = new SystemLine
                    {
                        Style = DashStyle.Solid,
                        Color = Tools.GetColorBySecurity(connectionType),
                        PointFrom =
                            new Point(connectedSolarSystem.LocationInMap.X - mapPositionX, connectedSolarSystem.LocationInMap.Y - mapPositionY),
                        PointTo = new Point(solarSystem.LocationInMap.X - mapPositionX, solarSystem.LocationInMap.Y - mapPositionY)
                    };

                    if ( connectedSolarSystem.Type == "D" || solarSystem.Type == "D" )
                    {
                        line.Style = DashStyle.DashDotDot;
                    }

                    _lines.Add(line);
                }
            }


        }

        private void ReDrawSolarSystemInformationPanel(string solarSystemName)
        {
            DrawSelectedSolarSystemInformation(solarSystemName);

            //ShowSignaturesForSystem(solarSystemName);
            FillSignaturesContainer(SpaceMap.GetSystem(solarSystemName).Signatures);

            containerMap.Refresh();
        }

        private void Event_ContainerMap_OnPaint(object sender, PaintEventArgs e)
        {
            //LocationSolarSystemName

            _commandsLog.InfoFormat("Paint for " + SelectedSolarSystemName + " " + DateTime.UtcNow.ToLongTimeString());
            lblUpdateTime.Text = "Paint for " + SelectedSolarSystemName  + " " + DateTime.UtcNow.ToLongTimeString();

            if (SelectedSolarSystemName == null) return;

            if (SelectedSolarSystemName != "" && SelectedSolarSystemName != null)
            {
                var selectedSolarSystem = SpaceMap.GetSystem(SelectedSolarSystemName);

                if (selectedSolarSystem != null)
                {
                    var rectangle = new Rectangle(selectedSolarSystem.LocationInMap.X - mapPositionX - 12,
                        selectedSolarSystem.LocationInMap.Y - mapPositionY - 12, 24, 24);

                    e.Graphics.DrawEllipse(new Pen(Color.Orange, 2), rectangle);
                    //e.Graphics.DrawArc(new Pen(new SolidBrush(Color.Black), 10), rectangle, 90, 180);
                    //e.Graphics.DrawArc(new Pen(new SolidBrush(Color.Goldenrod), 4), rectangle, 0, 180);
                }
            }

            if (LocationSolarSystemName != "")
            {
                var selectedSolarSystem = SpaceMap.GetSystem(LocationSolarSystemName);


                if (selectedSolarSystem != null)
                {
                    var rectangle = new Rectangle(selectedSolarSystem.LocationInMap.X - mapPositionX - 14,
                        selectedSolarSystem.LocationInMap.Y - mapPositionY - 14, 28, 28);

                    e.Graphics.DrawEllipse(new Pen(Color.DarkOrange, 2), rectangle);
                }

            }

            foreach (var systemLine in _lines)
            {
                var pen = new Pen(systemLine.Color, 1);
                pen.DashStyle = systemLine.Style;

                e.Graphics.DrawLine(pen, systemLine.PointFrom, systemLine.PointTo);
            }

            foreach (var visibleSystem in _systems)
            {
                var _solarSystem = SpaceMap.GetSystem(visibleSystem);

                if ( _solarSystem == null ) continue;

                var systemLabel = _solarSystem.Name;

                var _solarSystemInfo = Global.Space.GetSolarSystem(visibleSystem);

                if (Tools.IsWSpaceSystem(_solarSystem.Name)) systemLabel = systemLabel + "[C" + _solarSystemInfo.Class + "]";

                var drawFont = new Font("Verdana", 8, FontStyle.Bold);
                var drawBrushName = new SolidBrush(Color.Bisque);

                SizeF stringSize = new SizeF();
                stringSize = e.Graphics.MeasureString(systemLabel, drawFont);

                SizeF stringSize2 = new SizeF();
                stringSize2 = e.Graphics.MeasureString(_solarSystem.Name, drawFont);

                var drawFormat = new StringFormat();
                if ( _solarSystem.Type == "A" || _solarSystem.Type == "B" )
                {
                    e.Graphics.DrawString(_solarSystem.Name, drawFont, drawBrushName, _solarSystem.LocationInMap.X - mapPositionX + 2 - stringSize.Width / 2, _solarSystem.LocationInMap.Y - mapPositionY - 30, drawFormat);
                }

                if (Tools.IsWSpaceSystem(_solarSystem.Name))
                {
                    var drawBrush = new SolidBrush(Tools.GetColorBySolarSystem(systemLabel));
                    e.Graphics.DrawString("[C" + _solarSystemInfo.Class + "]", drawFont, drawBrush,
                        _solarSystem.LocationInMap.X - mapPositionX + 2 - stringSize.Width / 2 + stringSize2.Width, _solarSystem.LocationInMap.Y - mapPositionY - 30, drawFormat);
                }


                var rectangle = new Rectangle(_solarSystem.LocationInMap.X - mapPositionX - 10, _solarSystem.LocationInMap.Y - mapPositionY - 10, 20, 20);

                e.Graphics.FillEllipse(new SolidBrush(Tools.GetColorBySolarSystem(_solarSystemInfo.Security.ToString())), rectangle);
                //e.Graphics.FillEllipse(new SolidBrush(Color.Beige), rectangle);
                e.Graphics.DrawEllipse(new Pen(Color.DimGray, 1), rectangle);

                foreach (var pilot in SpaceMap.Pilotes)
                {
                    if (pilot.System == visibleSystem)
                    {
                        var controlName = pilot.Name.Replace(" ", "_");

                        var control = containerMap.Controls[controlName];

                        if (control != null)
                        {
                            var location = new Point(_solarSystem.LocationInMap.X - mapPositionX - 5, _solarSystem.LocationInMap.Y - mapPositionY - 5);

                            if (control.Location != location)
                            {
                                control.Location = location;
                            }
                        }
                    }
                }
            }

            if (LocationSolarSystemName != "")
            {
                var _solarSystem = SpaceMap.GetSystem(LocationSolarSystemName);


                if (_solarSystem != null)
                {
                    var systemLabel = _solarSystem.Name;

                    var _solarSystemInfo = Global.Space.GetSolarSystem(systemLabel);

                    if (Tools.IsWSpaceSystem(_solarSystem.Name)) systemLabel = systemLabel + "[C" + _solarSystemInfo.Class + "]";

                    var drawFont = new Font("Verdana", 8, FontStyle.Bold);
                    var drawBrushName = new SolidBrush(Color.Bisque);

                    SizeF stringSize = new SizeF();
                    stringSize = e.Graphics.MeasureString(systemLabel, drawFont);

                    SizeF stringSize2 = new SizeF();
                    stringSize2 = e.Graphics.MeasureString(_solarSystem.Name, drawFont);

                    var drawFormat = new StringFormat();
                    
                    e.Graphics.DrawString(_solarSystem.Name, drawFont, drawBrushName, _solarSystem.LocationInMap.X - mapPositionX + 2 - stringSize.Width / 2, _solarSystem.LocationInMap.Y - mapPositionY - 30, drawFormat);
                    

                    if (Tools.IsWSpaceSystem(_solarSystem.Name))
                    {
                        var drawBrush = new SolidBrush(Tools.GetColorBySolarSystem(systemLabel));
                        e.Graphics.DrawString("[C" + _solarSystemInfo.Class + "]", drawFont, drawBrush,
                            _solarSystem.LocationInMap.X - mapPositionX + 2 - stringSize.Width / 2 + stringSize2.Width, _solarSystem.LocationInMap.Y - mapPositionY - 30, drawFormat);
                    }


                    //var rectangle = new Rectangle(_solarSystem.LocationInMap.X - mapPositionX - 10, _solarSystem.LocationInMap.Y - mapPositionY - 10, 20, 20);

                    //e.Graphics.FillEllipse(new SolidBrush(Tools.GetColorBySolarSystem(_solarSystemInfo.Security.ToString())), rectangle);
                    ////e.Graphics.FillEllipse(new SolidBrush(Color.Beige), rectangle);
                    //e.Graphics.DrawEllipse(new Pen(Color.DimGray, 1), rectangle);

                    
                }

            }

            
        }

        private void Event_ContainerMap_OnMouseDown(object sender, MouseEventArgs e)
        {
            foreach (var visitedSolarSystem in SpaceMap.Systems)
            {
                var locationX = Math.Abs(visitedSolarSystem.LocationInMap.X - mapPositionX - e.X);
                var locationY = Math.Abs(visitedSolarSystem.LocationInMap.Y - mapPositionY - e.Y);

                if (locationX < 25 && locationY < 25)
                {
                    SelectedSolarSystemName = visitedSolarSystem.Name;

                    ReDrawSolarSystemInformationPanel(visitedSolarSystem.Name);
                }
            }
        }

        private string GetSystemByCoordinates(int X, int Y)
        {
            foreach (var visitedSolarSystem in SpaceMap.Systems)
            {
                var locationX = Math.Abs(visitedSolarSystem.LocationInMap.X - mapPositionX - X);
                var locationY = Math.Abs(visitedSolarSystem.LocationInMap.Y - mapPositionY - Y);

                if (locationX < 25 && locationY < 25)
                {
                    return visitedSolarSystem.Name;
                }
            }

            return null;
        }

        private void Event_ContainerMap_OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                
                var positionX = mapPositionX - screenSizeWidth / 2;
                var positionY = mapPositionY - screenSizeHeight / 2;

                //var selectedSystem = GetSystemByCoordinates(e.X, e.Y);

                //if (GetSystemByCoordinates(positionX, positionY) != null) return;

                if (GetSystemByCoordinates(e.X, e.Y) == null)
                {
                    mapPositionX = positionX + e.X;
                    mapPositionY = positionY + e.Y;
                    
                    DrawMap();

                    containerMap.Refresh();
                }
                return;
            }
            ////SelectedSolarSystemName
            var isNeedReLocateSolarSystem = true;

            foreach (var visitedSolarSystem in SpaceMap.Systems)
            {
                var locationX = Math.Abs(visitedSolarSystem.LocationInMap.X - e.X);
                var locationY = Math.Abs(visitedSolarSystem.LocationInMap.Y - e.Y);

                if (locationX < 25 && locationY < 25)
                {
                    isNeedReLocateSolarSystem = false;
                }
            }

            if (isNeedReLocateSolarSystem)
            {
                var system = SpaceMap.GetSystem(SelectedSolarSystemName);

                system.LocationInMap = new Point(e.X + mapPositionX, e.Y + mapPositionY);

                Global.MapApiFunctions.UpdateSolarSystemCoordinates(MapKey, SelectedSolarSystemName, Pilot, system.LocationInMap.X, system.LocationInMap.Y);

                DrawMap();

                //containerMap.Refresh();
            }

        }



        private BindingSource signaturesSource = new BindingSource();

        private void FillSignaturesContainer(List<CosmicSignature> signatures)
        {
            signaturesSource = new BindingSource();

            foreach (var cosmicSignature in signatures)
            {
                signaturesSource.Add(new CosmicSignature { Type = cosmicSignature.Type, Code = cosmicSignature.Code, Name = cosmicSignature.Name });
            }

            if (dataGridView1.Columns.Contains("Code")) dataGridView1.Columns.Remove("Code");
            if (dataGridView1.Columns.Contains("Name")) dataGridView1.Columns.Remove("Name");
            if (dataGridView1.Columns.Contains("Type")) dataGridView1.Columns.Remove("Type");

            DataGridViewColumn code = new DataGridViewTextBoxColumn();
            code.Width = 70;
            code.DataPropertyName = "Code";
            code.Name = "Code";
            dataGridView1.Columns.Add(code);

            DataGridViewColumn column = new DataGridViewTextBoxColumn();
            column.Width = 140;
            column.DataPropertyName = "Name";
            column.Name = "Name";
            dataGridView1.Columns.Add(column);

            var combo = new DataGridViewComboBoxColumn
            {
                Width = 80,
                FlatStyle = FlatStyle.Flat,
                DataSource = Enum.GetValues(typeof(SignatureType)),
                DataPropertyName = "Type",
                Name = "Type"
            };
            dataGridView1.Columns.Add(combo);

            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = signaturesSource;

            foreach (DataGridViewRow row in dataGridView1.Rows)
                if (row.Cells[2].Value.ToString() == "Cosmic Signature")
                {
                    row.DefaultCellStyle.BackColor = Color.DarkRed;
                }

            dataGridView1.ClearSelection();
        }


        private void Event_SignaturesGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentCell.ColumnIndex.Equals(0))
            {
                var code = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[1].Value;


            }

        }

        private void Event_LoadSignatures(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Clipboard.GetText())) return;

            var lines = Clipboard.GetText().Split(new[] { '\n' }, StringSplitOptions.None);

            var savedSignatures = LoadSignatures(LocationSolarSystemName);

            var signatures = new List<CosmicSignature>();

            const char tab = '\u0009';

            foreach (var line in lines)
            {
                Log.DebugFormat("[whlTravelHistory.Event_PasteSignatures] line = {0}", line);

                try
                {
                    var coordinates = line.Replace(tab.ToString(), "[---StarinForReplace---]");
                    var coordinate = coordinates.Split(new[] { @"[---StarinForReplace---]" }, StringSplitOptions.None)[0];
                    var type = coordinates.Split(new[] { @"[---StarinForReplace---]" }, StringSplitOptions.None)[2];
                    var name = coordinates.Split(new[] { @"[---StarinForReplace---]" }, StringSplitOptions.None)[1];
                    var label = coordinates.Split(new[] { @"[---StarinForReplace---]" }, StringSplitOptions.None)[3];
                    var m1 = Regex.Matches(coordinate, @"\d\d\d", RegexOptions.Singleline);

                    foreach (Match m in m1)
                    {
                        //listCosmicSifnatures.Items.Add("[" + coordinate + "] - " + name);
                        var signature = new CosmicSignature();
                        signature.Code = coordinate;
                        signature.SolarSystemName = LocationSolarSystemName;
                        //var type = record.Key.Split(new[] { @" - " }, StringSplitOptions.None)[1];
                        signature.Type = SignatureType.Unknown;

                        signature.Name = "Cosmic Signature";

                        if (type.ToUpper().IndexOf("ЧЕРВОТОЧИНА") > -1 || type.ToUpper().IndexOf("WORMHOLE") > -1)
                        {
                            signature.Name = type + " " + label;
                            signature.Type = SignatureType.WH;
                        }

                        if (type.ToUpper().IndexOf("ГАЗ") > -1 || type.ToUpper().IndexOf("GAS SITE") > -1)
                        {
                            signature.Name = type + " " + label;
                            signature.Type = SignatureType.Gas;
                        }

                        if (type.ToUpper().IndexOf("ДАННЫЕ") > -1 || type.ToUpper().IndexOf("DATA SITE") > -1)
                        {
                            signature.Name = type + " " + label;
                            signature.Type = SignatureType.Data;
                        }

                        if (type.ToUpper().IndexOf("АРТЕФАКТЫ") > -1 || type.ToUpper().IndexOf("RELIC SITE") > -1)
                        {
                            signature.Name = type + " " + label;
                            signature.Type = SignatureType.Relic;
                        }

                        //bool isNeedAddSignature = true;

                        if (signature.Type == SignatureType.Unknown)
                        {
                            foreach (var cosmicSignature in savedSignatures)
                            {
                                if (signature.Code == cosmicSignature.Code )
                                {
                                    if (cosmicSignature.Type != SignatureType.Unknown)
                                    {
                                        signature = cosmicSignature;
                                    }
                                }
                            }
                        }
                        signatures.Add(signature);
                    }
                }
                catch (Exception ex)
                {
                    Log.ErrorFormat("[whlTravelHistory.Event_PasteSignatures] Critical error = {0}", ex);
                }
            }

            FillSignaturesContainer(signatures);

            //SaveSignatures(signatures, LocationSolarSystemName);
            if(signatures.Count > 0){
                Global.MapApiFunctions.PublishSignatures(Pilot, SpaceMap.Key, LocationSolarSystemName, signatures);
                UpdateSpaceMap();
            }

        }


        private List<CosmicSignature> LoadSignatures(string solarSystemName)
        {
            var fileName = @"Data/Signatures/" + solarSystemName + ".csv";

            var signatures = new List<CosmicSignature>();

            try
            {
                if (File.Exists(fileName) == false) return signatures;

                using (var sr = new StreamReader(fileName))
                {
                    var records = new CsvReader(sr).GetRecords<CosmicSignature>();

                    signatures.AddRange(records);
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("[whlTravelHistory.LoadTravelHistorySignatures] Critical error = {0}", ex);
            }

            return signatures;

        }

        public void Relocation(PilotEntity pilot, string systemFrom, string systemTo, bool isNeedRefresh)
        {

            if (InvokeRequired)
            {
                Invoke(new Action(() => Relocation(pilot, systemFrom, systemTo, isNeedRefresh)));
            }

            if (SpaceMap == null) return;

            _commandsLog.InfoFormat("ChangeCurrentLocation for " + systemTo + " from " + systemFrom);

            LocationSolarSystemName = systemTo;

            SelectedSolarSystemName = systemTo;

            PreviousLocationSolarSystemName = systemFrom;

            UpdateSpaceMap();

            

            DrawMap();

            _commandsLog.InfoFormat("Command paint for " + SelectedSolarSystemName + " " + DateTime.UtcNow.ToLongTimeString());

            containerMap.Refresh();
            //Invoke(new Action(() => containerMap.Refresh()));

            //Invoke(new Action(() => DrawSelectedSolarSystemInformation(systemTo)));
            DrawSelectedSolarSystemInformation(systemTo);
        }

        private void Event_ContainerMap_OnMouseMove(object sender, MouseEventArgs e)
        {
            //pilotesInSolarSystemTooltip
        }

        private void Event_ContainerMap_OnResize(object sender, EventArgs e)
        {
            
        }

        public void Event_Map_OnResize()
        {
            if(screenSizeWidth + screenSizeHeight == containerMap.Width + containerMap.Height) return;

            screenSizeWidth = containerMap.Width;
            screenSizeHeight = containerMap.Height;

            var currentSystem = SpaceMap.GetSystem(SelectedSolarSystemName);

            if (currentSystem == null) return;

            mapPositionX = currentSystem.LocationInMap.X - screenSizeWidth / 2;
            mapPositionY = currentSystem.LocationInMap.Y - screenSizeHeight / 2;

            Event_Map_OnResize();

            UpdateSpaceMap();

            DrawMap();

            containerMap.Refresh();
        }

        private void ejButton3_Click(object sender, EventArgs e)
        {
            Global.MapApiFunctions.DeleteSolarSystem(SpaceMap.Key, SelectedSolarSystemName, Global.Pilots.Selected.Name);
        }

        private void Event_OpenMapSettings(object sender, EventArgs e)
        {
            cmdMapSettings.ForeColor = Color.DarkGoldenrod;
            cmdSystemInformation.ForeColor = Color.Silver;

            var windowMapSettings = new windowMapSettings(SpaceMap);

            windowMapSettings.ShowDialog(this);
        }

        private void ejButton4_Click(object sender, EventArgs e)
        {

        }
    }
}
