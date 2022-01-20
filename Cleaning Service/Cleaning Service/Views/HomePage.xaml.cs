using Cleaning_Service.Models;
using Cleaning_Service.ViewModels;
using Plugin.CloudFirestore;
using Plugin.FirebaseAuth;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class HomePage : ContentPage
    {
        public MapPageViewModel map_vm;
        private Pin pin;

        public HomePage()
        {
            InitializeComponent();
            map_vm = new MapPageViewModel();
            BindingContext = map_vm;
            g_map.CameraIdled += G_map_CameraIdled;
            g_map.PinClicked += G_map_PinClicked;
            InitMap();
            Loadmaids();
        }

        private async void G_map_PinClicked(object sender, PinClickedEventArgs e)
        {
            if(e.Pin.Tag.ToString() != CrossFirebaseAuth.Current.Instance.CurrentUser.Uid)
            {
                await PopupNavigation.Instance.PushAsync(new RequestMaidPage(e.Pin.Tag.ToString()));
            }
        }

        private readonly ObservableCollection<User> users = new ObservableCollection<User>();
        public ObservableCollection<User> Users { get { return users; } }
        private void Loadmaids()
        {
            
            CrossCloudFirestore
                .Current
                .Instance
                .Collection("USERS")
                .WhereEqualsTo("Role", "Maid")
                .AddSnapshotListener(async (value, error) =>
                {
                    if (!value.IsEmpty)
                    {
                        foreach (var item in value.DocumentChanges)
                        {
                            User u = new User();
                            switch (item.Type)
                            {
                                case DocumentChangeType.Added:
                                    u = item.Document.ToObject<User>();
                                    users.Add(u);
                                    if (u.Latitude.Trim() != null && u.Longitude.Trim() != null)
                                    {
                                        System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");
                                        cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
                                        System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
                                        var pin = await MapPin(double.Parse(u.Latitude.Trim()), double.Parse(u.Longitude.Trim()), u);
                                        g_map.Pins.Add(pin);
                                    }
                                    break;
                                case DocumentChangeType.Modified:
                                    break;
                                case DocumentChangeType.Removed:
                                    break;
                            }
                        }
                    }
                });
        }

        



        private void G_map_CameraIdled(object sender, CameraIdledEventArgs e)
        {

        }

        private async void InitMap()
        {
            System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");
            cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
            var location = await Geolocation.GetLastKnownLocationAsync();
            if (location != null)
            {
                pin = await MapPinCurrentUser(location.Latitude, location.Longitude);
                g_map.Pins.Add(pin);
                g_map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(location.Latitude, location.Longitude), Distance.FromMeters(500)));
            }
        }
        private async Task<Pin> MapPin(double latitude, double longitude, User user)
        {
            return new Pin()
            {
                Address = await GetAddress(new Location(latitude, longitude)),
                Type = PinType.Place,
                Label = $"{user.FirstName} {user.LastName}",
                Position = new Position(latitude, longitude),

                Tag = user.Id,
                Icon = Device.RuntimePlatform == Device.Android ?
                                    BitmapDescriptorFactory.FromBundle("maid_icon.png") :
                                    BitmapDescriptorFactory.FromView(new Image { Source = "maid_icon.png", WidthRequest = 35, HeightRequest = 35 })
            };
        }
        private async Task<Pin> MapPinCurrentUser(double latitude, double longitude)
        {
            return new Pin()
            {
                Address = await GetAddress(new Location(latitude, longitude)),
                Type = PinType.Place,
                Label = "lbl",
                Position = new Position(latitude, longitude),
                Tag = CrossFirebaseAuth.Current.Instance.CurrentUser.Uid
            };
        }
        private async Task<string> GetAddress(Location latlang)
        {
            Geocoder geocode = new Geocoder();
            var address = await geocode.GetAddressesForPositionAsync(new Position(latlang.Latitude, latlang.Longitude));
            return address.FirstOrDefault().ToString();
        }
        private void BtnLogout_Clicked(object sender, EventArgs e)
        {
            Plugin.FirebaseAuth
                .CrossFirebaseAuth
                .Current
                .Instance
                .SignOut();
        }
    }
}