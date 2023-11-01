using GMap.NET.WindowsPresentation;
using GMap.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DA.Markers;
using System.Threading;
using System.ComponentModel;
using System.Windows.Media.Media3D;


namespace DA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        private static AutoResetEvent _wait = new AutoResetEvent(false);
        public bool Paused;
        
        public MainWindow()
        {
            InitializeComponent();
            
        }
        
       

        private void mapView_Loaded(object sender, RoutedEventArgs e)
        {
            
            Paused = true;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache;
            // choose your provider here
            mapView.MapProvider = GMap.NET.MapProviders.OpenStreetMapProvider.Instance;
            mapView.MinZoom = 2;
            mapView.MaxZoom = 17;
            mapView.Zoom = 15;
            mapView.MapPoint = new Point(50.96207262263082, -1.4255905415780727); //starting point

            var earth = 6378.137;  //radius of the earth in kilometer
            double pi = Math.PI;
            double m = (1 / ((2 * pi / 360) * earth)) / 1000;  //1 meter in degree

            //finding points 500 meters away for testing
            var new_latitude = 50.96207262263082 + (500 * m);
            var new_longitude = -1.4255905415780727 + (500 * m);
            PointLatLng pos = new PointLatLng(50.96207262263082, -1.4255905415780727);
            PointLatLng posplane = new PointLatLng(new_latitude, -1.4255905415780727);
            //PointLatLng pos2 = new PointLatLng(50.96207262263082, new_longitude);

            //sets the plane marker
            GMapMarker marker = new GMapMarker(posplane);
            {
                marker.Shape = new CustomMarkerPlane(this, marker);
            }
            mapView.Markers.Add(marker);
            


           /* GMapMarker markerdebug = new GMapMarker(posplane);
            {       
                markerdebug.Shape = new Ellipse
                {
                    Width = 5,
                    Height = 5,
                    Stroke = Brushes.Red,
                    StrokeThickness = 1.5
                };
            }
            mapView.Markers.Add(markerdebug);*/

       /*     GMapMarker markerdebug2 = new GMapMarker(pos2);
            {
                markerdebug2.Shape = new Ellipse
                {
                    Width = 5,
                    Height = 5,
                    Stroke = Brushes.Red,
                    StrokeThickness = 1.5
                };
            }
            mapView.Markers.Add(markerdebug2);*/

           /* GMapMarker markerdebug3 = new GMapMarker(pos);
            {
                markerdebug3.Shape = new Ellipse
                {
                    Width = 5,
                    Height = 5,
                    Stroke = Brushes.Red,
                    StrokeThickness = 1.5
                };
            }
            mapView.Markers.Add(markerdebug3);*/

           // PointLatLng startingpos = new PointLatLng();
            /*GMapMarker circle = new GMapMarker(startingpos);
            {
                
                circle.Shape = new Ellipse
                {
                    Width = 200,
                    Height = 200,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1.5,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    
                    
                };
                
            }
            mapView.Markers.Add(circle);*/
           // PointLatLng startingpos3 = new PointLatLng();

           //sets the starting point
            PointLatLng startingpos2 = new PointLatLng(50.96207262263082, -1.4255905415780727);

            // makes a list of points for the plane to follow
            List<PointLatLng> gpollist = new List<PointLatLng>();

            //creates the path with a desired starting position, distance and smoothness of path
            gpollist = createcircle(startingpos2, 500, 20000);
            // CreateCircle(50.96207262263082, -1.4255905415780727, 100);





            /*List<PointLatLng> points = new List<PointLatLng>();
            points.Add(new PointLatLng(50.96207262263082, -1.4255905415780727));
            points.Add(new PointLatLng(550.96207262263082, -1.4255905415780727));
            GMapPolygon polygons = new GMapPolygon(points);
            Drawing.ReferenceEquals(polygons, mapView.MapProvider = GMap.NET.MapProviders.OpenStreetMapProvider.Instance);*/

            // lets the map use the mousewheel to zoom
            mapView.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            //mapView.MouseWheelZoomEnabled = false;
            // lets the user drag the map
             mapView.CanDragMap = true;
            // lets the user drag the map with the left mouse button
            mapView.DragButton = MouseButton.Left;
            //PlanePath p = new PlanePath(gpollist);

            //moves the plane
            var PPlane = new Thread(PlanePath);
            PPlane.IsBackground = true;
            PPlane.Start(gpollist);

        }

        private List<PointLatLng> createcircle(PointLatLng point, double radius, int segments)
        {

            List<PointLatLng> gpollist = new List<PointLatLng>();

            double seg = Math.PI * 2 / segments;
            var r_earth = 6378137;


            for (int i = 0; i < segments; i++)
            {
                double theta = seg * i;
                double dy = Math.Cos(theta) * radius;
                double dx = Math.Sin(theta) * radius;
                double new_latitude = point.Lat + (dy / r_earth) * (180 / Math.PI);
                double new_longitude = point.Lng + (dx / r_earth) * (180 / Math.PI) / Math.Cos(point.Lat * Math.PI / 180); //confusing maths to figure out each point on the path


                PointLatLng gpoi = new PointLatLng(new_latitude, new_longitude);
                
                gpollist.Add(gpoi);
            }
            GMapPolygon gpol = new GMapPolygon(gpollist);
            //GMapRoute gMapRoute = new GMapRoute(gpollist);
            Path path = new Path();
            path.StrokeThickness = 1.5;
            path.Stroke = Brushes.DarkBlue;
            path.Effect = null;

            gpol.Shape = path;
           // gMapRoute.Shape = path;
            mapView.Markers.Add(gpol);
            //mapView.Markers.Add(gMapRoute);
            return gpollist;
        }

      

      /*  private void CreateCircle(Double lat, Double lon, double radius)
        {
            PointLatLng point = new PointLatLng(lat, lon);
            int segments = 1000;

            List<PointLatLng> gpollist = new List<PointLatLng>();

            for (int i = 0; i < segments; i++)
                gpollist.Add(FindPointAtDistanceFrom(point, i, radius / 1000));

            GMapPolygon gpol = new GMapPolygon(gpollist);
            GMapRoute gMapRoute = new GMapRoute(gpollist);
            Path path = new Path();
            path.StrokeThickness = 1;
            path.Stroke = Brushes.DarkBlue;
            path.Effect = null;

            gpol.Shape = path;
            mapView.Markers.Add(gMapRoute);
        }

        public static GMap.NET.PointLatLng FindPointAtDistanceFrom(GMap.NET.PointLatLng startPoint, double initialBearingRadians, double distanceKilometres)
        {
            const double radiusEarthKilometres = 6371.01;
            var distRatio = distanceKilometres / radiusEarthKilometres;
            var distRatioSine = Math.Sin(distRatio);
            var distRatioCosine = Math.Cos(distRatio);

            var startLatRad = DegreesToRadians(startPoint.Lat);
            var startLonRad = DegreesToRadians(startPoint.Lng);

            var startLatCos = Math.Cos(startLatRad);
            var startLatSin = Math.Sin(startLatRad);

            var endLatRads = Math.Asin((startLatSin * distRatioCosine) + (startLatCos * distRatioSine * Math.Cos(initialBearingRadians)));

            var endLonRads = startLonRad + Math.Atan2(
                          Math.Sin(initialBearingRadians) * distRatioSine * startLatCos,
                          distRatioCosine - startLatSin * Math.Sin(endLatRads));

            return new GMap.NET.PointLatLng(RadiansToDegrees(endLatRads), RadiansToDegrees(endLonRads));
        }

        public static double DegreesToRadians(double degrees)
        {
            const double degToRadFactor = Math.PI / 180;
            return degrees * degToRadFactor;
        }

        public static double RadiansToDegrees(double radians)
        {
            const double radToDegFactor = 180 / Math.PI;
            return radians * radToDegFactor;
        }*/



        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
           
            if(PlayPause.Content.ToString() == "Play")
            {
                PlayPause.Content = "Pause";
                Paused = false;
                _wait.Set();
            }
            else
            {
                PlayPause.Content = "Play";
                Paused = true;
                
            }
        }

        public void PlanePath(object list)
        {
            
            List<PointLatLng> gpollist = new List<PointLatLng>();
            gpollist = (List<PointLatLng>)list;
            RotateTransform RT = new RotateTransform();
            double ratio = 360 / gpollist.Count;
            

            for (; ; )
            {
                int i = 0;
                foreach (PointLatLng point in gpollist)
                {
                    if (Paused == true)
                    {
                        _wait.WaitOne();//Pause the loop until unpause.

                    }
                    this.Dispatcher.Invoke(() =>
                    {
                        mapView.Markers[0].Position = point;
                        
                        RT = new RotateTransform(i*ratio);
                        //could change the rotation of the plane icon here using RT
                    });
                  //  Console.WriteLine("Playing");
                    Thread.Sleep(1);
                    i++;
                }
            }
        }

        void MainWindow_Closing(object sender, EventArgs e)
        {
            //MessageBox.Show("Closing called");
            
        }



    }
}
