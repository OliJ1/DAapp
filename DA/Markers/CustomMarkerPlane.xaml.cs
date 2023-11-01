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
using GMap.NET.WindowsPresentation;
using GMap.NET;


namespace DA.Markers
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class CustomMarkerPlane
    {
        GMapMarker Marker;
        MainWindow MainWindow;
        PlaneAngle angle = new PlaneAngle { Angle = 0 };
        public CustomMarkerPlane(MainWindow window, GMapMarker marker)
        {
            InitializeComponent();
            this.MainWindow = window;
            this.Marker = marker;
            this.Loaded += new RoutedEventHandler(CustomMarkerPlane_Loaded);
            this.SizeChanged += new SizeChangedEventHandler(CustomMarkerplane_SizeChanged);
            this.DataContext = angle;
        }

        void CustomMarkerPlane_Loaded(object sender, RoutedEventArgs e)
        {
            if (icon.Source.CanFreeze)
            {
                icon.Source.Freeze();
            }
        }

        void CustomMarkerplane_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Marker.Offset = new Point(-e.NewSize.Width / 2, -e.NewSize.Height);
        }

        public class PlaneAngle
        {
            private double anglevalue;

            public double Angle
            {
                get { return anglevalue; }
                set { anglevalue = value; }
            }
        }
    }
}
