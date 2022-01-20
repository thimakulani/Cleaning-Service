using Plugin.CloudFirestore;
using Plugin.FirebaseAuth;
using Rg.Plugins.Popup.Services;
using System.Collections.Generic;

namespace Cleaning_Service.Views
{
    public partial class RequestMaidPage
    {
        private string id;
        public RequestMaidPage(string id)
        {
            this.id = id;
            InitializeComponent();
        }

        private async void ImgLocation_Clicked(object sender, System.EventArgs e)
        {
            var map = new MapDialog();
            map.AddressChanged += Map_AddressChanged;
            await PopupNavigation.Instance.PushAsync(map);
        }
        private string lat, lon;

        private async void BtnSubmit_Clicked(object sender, System.EventArgs e)
        {
            BtnSubmit.IsEnabled = false;
            if (lat == null && lon == null)
            {
                await DisplayAlert("Warning", "Price is required", "Got it");
                BtnSubmit.IsEnabled = true;
                return;
            }
            if (TxtPlaceAddress.Text == null)
            {
                await DisplayAlert("Warning", "Address is required", "Got it");
                BtnSubmit.IsEnabled = true;
                return;
            }
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "Lat", lat },
                { "Lon", lon },
                { "Mid", id },
                { "Uid", CrossFirebaseAuth.Current.Instance.CurrentUser.Uid },
                { "Address", TxtPlaceAddress.Text },
                { "Price", InputPrice.Text },
                { "StartDate", SelectedDates.Date.ToString("ddd/MMM/yyyy") },
                { "Status", "Waiting" }
            };
            await CrossCloudFirestore
                .Current
                .Instance
                .Collection("REQUESTS")
                .AddAsync(data);
            await PopupNavigation.Instance.PopAsync();
            BtnSubmit.IsEnabled = true;

        }

        private void Map_AddressChanged(object sender, MapDialog.AddressEventHandler e)
        {
            TxtPlaceAddress.Text = e.Address;
            lat = e.Latitude;
            lon = e.Longitude;
        }
    }
}