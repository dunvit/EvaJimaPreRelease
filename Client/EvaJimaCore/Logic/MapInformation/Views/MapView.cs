using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Timers;
using System.Windows.Forms;
using EvaJimaCore;
using EveJimaCore.BLL.Map;
using EveJimaUniverse;
using log4net;

namespace EveJimaCore.Logic.MapInformation
{
    public partial class MapView : UserControl
    {
        readonly ILog _commandsLog = LogManager.GetLogger("Errors");

        private readonly Dictionary<string, StarSystemEntity> _systemsInformation = new Dictionary<string, StarSystemEntity>();

        private Point ScreenCenter { get; set; }

        private Point MapPosition { get; set; }

        private System.Timers.Timer aTimer;

        bool isDragging;

        public event Action<string> SelectSolarSystem;

        public event Action<Point, string> RelocateSolarSystem;

        public MapView()
        {
            InitializeComponent();

            aTimer = new System.Timers.Timer();
            aTimer.Elapsed += Event_Refresh;
            aTimer.Interval = 1000;
            aTimer.Enabled = true;

            
            MouseUp += Map_MouseUp;
            MouseDown += Map_MouseDown;
        }

        private bool isRelocateSystem;
        private string relocatedSystem;

        private Point relocatedSystemStartPosition = new Point(0, 0);
        private Point _drugAndDropStartPosition = new Point(0, 0);

        private void Map_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
            aTimer.Interval = 100;

            if (e.Button == MouseButtons.Right && isRelocateSystem)
            {
                isRelocateSystem = false;


                if (RelocateSolarSystem != null)
                {
                    RelocateSolarSystem(Global.Pilots.Selected.SpaceMap.GetSystem(relocatedSystem).LocationInMap, relocatedSystem);
                }

                relocatedSystem = string.Empty;
                relocatedSystemStartPosition = new Point(0, 0);
            }
        }

        private void Map_MouseDown(object sender, MouseEventArgs e)
        {
            var mapPoint = new Point(MapPosition.X + e.X, MapPosition.Y + e.Y);

            if (e.Button == MouseButtons.Left)
            {
                if (IsSelectedSolarSystem(mapPoint)) return;

                isDragging = true;
                _drugAndDropStartPosition = new Point((Width / 2) - e.X, (Height / 2) - e.Y);

            }

            if (e.Button == MouseButtons.Right)
            {
                var selectedSystem = GetSolarSystem(mapPoint);

                if (selectedSystem != string.Empty)
                {
                    relocatedSystem = selectedSystem;
                    isRelocateSystem = true;
                    relocatedSystemStartPosition = new Point(e.X, e.Y);

                    Global.Pilots.Selected.SpaceMap.SelectedSolarSystemName = selectedSystem;

                    if (Global.Pilots.Selected.SpaceMap.SelectedSolarSystemName != null) SelectSolarSystem(Global.Pilots.Selected.SpaceMap.SelectedSolarSystemName);
                }


            }
        }

        private string GetSolarSystem(Point mapPoint)
        {
            foreach (var visitedSolarSystem in Global.Pilots.Selected.SpaceMap.Systems)
            {
                var locationX = Math.Abs(visitedSolarSystem.LocationInMap.X - mapPoint.X);
                var locationY = Math.Abs(visitedSolarSystem.LocationInMap.Y - mapPoint.Y);

                if (locationX < 25 && locationY < 25)
                {
                    return visitedSolarSystem.Name;
                }
            }

            return string.Empty;
        }

        private bool IsSelectedSolarSystem(Point mapPoint)
        {
            foreach (var visitedSolarSystem in Global.Pilots.Selected.SpaceMap.Systems)
            {
                var locationX = Math.Abs(visitedSolarSystem.LocationInMap.X - mapPoint.X);
                var locationY = Math.Abs(visitedSolarSystem.LocationInMap.Y - mapPoint.Y);

                if (locationX < 25 && locationY < 25)
                {
                    Global.Pilots.Selected.SpaceMap.SelectedSolarSystemName = visitedSolarSystem.Name;

                    if (Global.Pilots.Selected.SpaceMap.SelectedSolarSystemName != null) SelectSolarSystem(Global.Pilots.Selected.SpaceMap.SelectedSolarSystemName);

                    return true;
                }
            }

            return false;
        }

        private void Event_Refresh(object sender, ElapsedEventArgs e)
        {
            RefreshMap();

        }

        private void RefreshMap()
        {
            try
            {
                if (InvokeRequired) 
                {
                    Invoke(new MethodInvoker(RefreshMap));
                    return;
                }

                Refresh();
            }
            catch
            {

            }
        }

        private Map SpaceMap { get; set; }

        public void ForceRefresh(Map spaceMap)
        {
            try
            {
                SpaceMap = spaceMap;

                if (spaceMap == null) return;

                ScreenCenter = SpaceMap.GetSystem(SpaceMap.LocationSolarSystemName).LocationInMap;

                RecalculateOffsetPositions(ScreenCenter);
            }
            catch (Exception ex)
            {
                _commandsLog.ErrorFormat("[MapView.ForceRefresh] Critical error {0}", ex);
            }


        }

        private bool isPainting;

        private void Event_Paint(object sender, PaintEventArgs e)
        {
            if (isPainting) return;

            isPainting = true;

            try
            {
                if(isDragging)
                {
                    var coordinates = PointToClient(Cursor.Position);

                    var screenCenter = new Point(ScreenCenter.X + coordinates.X - Width / 2, ScreenCenter.Y + coordinates.Y - Height / 2);

                    RecalculateOffsetPositions(screenCenter);
                }

                var rectangleMapCenter = new Rectangle(Width / 2 - 0,
                    Height / 2 - 0, 3, 3);

                e.Graphics.DrawEllipse(new Pen(Color.Red, 2), rectangleMapCenter);

                lblUpdateTime.Text = @"Updated at " + DateTime.UtcNow.ToLongTimeString();

                #region Draw selected solar system aura

                foreach(var solarSystem in SpaceMap.Systems)
                {
                    if(solarSystem.Name == null || solarSystem.Name != SpaceMap.SelectedSolarSystemName) continue;

                    var rectangle = new Rectangle(solarSystem.LocationInMap.X - MapPosition.X - 14,
                        solarSystem.LocationInMap.Y - MapPosition.Y - 14, 28, 28);

                    e.Graphics.DrawEllipse(new Pen(Color.DarkOrange, 2), rectangle);

                }

                #endregion

                #region Draw location solar system aura

                foreach(var solarSystem in SpaceMap.Systems)
                {
                    if(solarSystem.Name == null || solarSystem.Name != SpaceMap.LocationSolarSystemName) continue;

                    var rectangle = new Rectangle(solarSystem.LocationInMap.X - MapPosition.X - 12,
                        solarSystem.LocationInMap.Y - MapPosition.Y - 12, 24, 24);

                    e.Graphics.DrawEllipse(new Pen(Color.DarkGreen, 2), rectangle);

                }

                #endregion

                #region Draw connection lines

                foreach(var solarSystem in SpaceMap.Systems)
                {
                    if(solarSystem.Name == null) continue;
                    if(solarSystem.IsDeleted) continue;
                    if(solarSystem.IsHidden) continue;

                    foreach(var connection in solarSystem.Connections)
                    {
                        var connectedSolarSystem = SpaceMap.Systems.FirstOrDefault(system => system.Name == connection);

                        if(connectedSolarSystem.Name == null) continue;
                        if(connectedSolarSystem.IsDeleted) continue;

                        var pen = new Pen(Color.Gray, 1);
                        pen.DashStyle = DashStyle.Solid;

                        var pointFrom = new Point(solarSystem.LocationInMap.X - MapPosition.X, solarSystem.LocationInMap.Y - MapPosition.Y);
                        var pointTo = new Point(connectedSolarSystem.LocationInMap.X - MapPosition.X,
                            connectedSolarSystem.LocationInMap.Y - MapPosition.Y);

                        e.Graphics.DrawLine(pen, pointFrom, pointTo);
                    }
                }

                #endregion

                #region Draw solar systems and names

                foreach(var solarSystem in SpaceMap.Systems)
                {
                    if(solarSystem.Name == null) continue;
                    if(solarSystem.IsDeleted) continue;
                    if(solarSystem.IsHidden) continue;

                    var systemLabel = solarSystem.Name;

                    if(_systemsInformation.ContainsKey(solarSystem.Name) == false)
                        _systemsInformation.Add(solarSystem.Name, Global.Space.GetSolarSystem(solarSystem.Name));


                    if(Tools.IsWSpaceSystem(solarSystem.Name)) systemLabel = systemLabel + "[C" + _systemsInformation[solarSystem.Name].Class + "]";

                    var drawFont = new Font("Verdana", 8, FontStyle.Bold);
                    var drawBrushName = new SolidBrush(Tools.GetColorBySolarSystem(_systemsInformation[solarSystem.Name].Security.ToString()));

                    if(Tools.IsWSpaceSystem(solarSystem.Name))
                    {
                        drawBrushName = new SolidBrush(Tools.GetColorBySolarSystem("C" + _systemsInformation[solarSystem.Name].Class));
                    }

                    var stringSize = e.Graphics.MeasureString(systemLabel, drawFont);

                    var stringSize2 = e.Graphics.MeasureString(solarSystem.Name, drawFont);

                    var drawFormat = new StringFormat();


                    if(solarSystem.Type == "A" || solarSystem.Type == "B" || solarSystem.Type == "C")
                    {
                        e.Graphics.DrawString(solarSystem.Name, drawFont, drawBrushName,
                            solarSystem.LocationInMap.X - MapPosition.X + 2 - stringSize.Width / 2, solarSystem.LocationInMap.Y - MapPosition.Y - 30,
                            drawFormat);
                    }

                    if(Tools.IsWSpaceSystem(solarSystem.Name))
                    {
                        var drawBrush = new SolidBrush(Tools.GetColorBySolarSystem("C" + _systemsInformation[solarSystem.Name].Class));
                        e.Graphics.DrawString("[C" + _systemsInformation[solarSystem.Name].Class + "]", drawFont, drawBrush,
                            solarSystem.LocationInMap.X - MapPosition.X + 2 - stringSize.Width / 2 + stringSize2.Width,
                            solarSystem.LocationInMap.Y - MapPosition.Y - 30, drawFormat);
                    }

                    var rectangle = new Rectangle(solarSystem.LocationInMap.X - MapPosition.X - 8, solarSystem.LocationInMap.Y - MapPosition.Y - 8, 16,
                        16);

                    e.Graphics.FillEllipse(new SolidBrush(Tools.GetColorBySolarSystem(_systemsInformation[solarSystem.Name].Security.ToString())),
                        rectangle);
                    e.Graphics.DrawEllipse(new Pen(Color.DimGray, 1), rectangle);
                }

                #endregion
            }
            catch(Exception ex)
            {

                //var someValue = SpaceMap.Systems;
                //var defaultValue = SpaceMap.Systems;

                ////result будет 23, если someValue - null
                //var result = someValue ?? defaultValue;
            }
            finally
            {
                isPainting = false;
            }
        }

        private void RecalculateOffsetPositions(Point screenCenter)
        {
            MapPosition = new Point(screenCenter.X - Width / 2, screenCenter.Y - Height / 2);
        }

        private void Event_MouseMove(object sender, MouseEventArgs e)
        {
            //lblMouseMoveCoordinates.Text = "Coordinates Local x=" + e.X + " y=" + e.Y + Environment.NewLine
            //    + "Coordinates Local x=" + (e.X + MapPosition.X) + " y=" + (e.Y + MapPosition.Y) +
            //    Environment.NewLine + "relocatedSystemStartPosition Local x=" + relocatedSystemStartPosition.X + " y=" + relocatedSystemStartPosition.Y;
            //relocatedSystemStartPosition
            if (isRelocateSystem && relocatedSystem != string.Empty)
            {
                Global.Pilots.Selected.SpaceMap.GetSystem(relocatedSystem).LocationInMap = new Point(e.X + MapPosition.X, e.Y + MapPosition.Y);
            }

            if (isDragging)
            {
                //var screenCenter = new Point(MapPosition.X + e.X + drugAndDropStartPosition.X, MapPosition.Y + e.Y + drugAndDropStartPosition.Y);

                //RecalculateOffsetPositions(screenCenter);

                //ScreenCenter = screenCenter;
            }
        }

        public void CentreScreenBySelectedSystem()
        {
            ScreenCenter = Global.Pilots.Selected.SpaceMap.Systems.FirstOrDefault(system => system.Name == Global.Pilots.Selected.SpaceMap.SelectedSolarSystemName).LocationInMap;

            RecalculateOffsetPositions(ScreenCenter);

            Refresh();
        }

        public void CentreScreenByLocationSystem()
        {
            ScreenCenter = Global.Pilots.Selected.SpaceMap.Systems.FirstOrDefault(system => system.Name == Global.Pilots.Selected.SpaceMap.LocationSolarSystemName).LocationInMap;

            RecalculateOffsetPositions(ScreenCenter);

            Refresh();
        }
    }
}
