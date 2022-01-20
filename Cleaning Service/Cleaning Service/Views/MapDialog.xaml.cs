using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.Xaml;

namespace Cleaning_Service.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapDialog 
    {
        public MapDialog()
        {
            InitializeComponent();

            g_maps.CameraIdled += G_maps_CameraIdled;
            LoadMap();
        }
        private async void LoadMap()
        {
            var location = await Geolocation.GetLastKnownLocationAsync();

            if (location != null)
            {
                g_maps.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(location.Latitude, location.Longitude), Distance.FromMeters(500)));
                g_maps.MyLocationButtonClicked += G_maps_MyLocationButtonClicked;
                g_maps.MyLocationEnabled = true;
                TxTAddress.Text = await GetAddress(location);
            }

        }

        private void G_maps_MyLocationButtonClicked(object sender, MyLocationButtonClickedEventArgs e)
        {
            
        }

        private async Task<string> GetAddress(Location latlang)
        {
            Geocoder geocode = new Geocoder();
            var address = await geocode.GetAddressesForPositionAsync(new Position(latlang.Latitude, latlang.Longitude));
            return address.FirstOrDefault().ToString();
        }
        private string lat;
        private string lon;

        private async void G_maps_CameraIdled(object sender, Xamarin.Forms.GoogleMaps.CameraIdledEventArgs e)
        {
            System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");
            cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;

            g_maps.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(e.Position.Target.Latitude, e.Position.Target.Longitude), Distance.FromMeters(100)));
            lat = e.Position.Target.Latitude.ToString();
            lon = e.Position.Target.Longitude.ToString();

            var l = new Location(e.Position.Target.Latitude, e.Position.Target.Longitude);

            TxTAddress.Text = await GetAddress(l);
        }
        public event EventHandler<AddressEventHandler> AddressChanged;
        public class AddressEventHandler: EventArgs
        {
            public string Address { get; set; }
            public string Latitude { get; set; } 
            public string Longitude { get; set; }
        }
        private void BtnSubmit_Clicked(object sender, EventArgs e)
        {
            AddressChanged.Invoke(this, new AddressEventHandler { Address = TxTAddress.Text, Latitude = lat, Longitude = lon });
            PopupNavigation.Instance.PopAsync();
        }
    }
}