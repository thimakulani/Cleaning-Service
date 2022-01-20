using Cleaning_Service.Models;
using Cleaning_Service.Views;
using Cleaning_Service.MaidApp;
using Plugin.CloudFirestore;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Cleaning_Service.MaidView;

namespace Cleaning_Service
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            
            //MainPage = new MainPage();

            //MainPage = new MyFlyoutPage(new FlyoutPage());
        }

        private async void Instance_AuthState(object sender, Plugin.FirebaseAuth.AuthStateEventArgs e)
        {
            if (e.Auth.CurrentUser != null)
            {

                var query = await CrossCloudFirestore
                 .Current
                 .Instance
                 .Collection("USERS")
                 .Document(e.Auth.CurrentUser.Uid)
                 .GetAsync();
                if (query.Exists)
                {
                    var user = query.ToObject<User>();
                    if (user.Role == "House Hold")
                    {
                        MainPage = new AppShell();
                    }
                    if (user.Role == "Maid")
                    {
                        MainPage = new MaidAppShell();
                    }
                }
            }
            else
            {
                MainPage = new LoginPage();
            }
        }

        protected override void OnStart()
        {
            Plugin.FirebaseAuth.CrossFirebaseAuth.Current
                    .Instance
                    .AuthState += Instance_AuthState;
        }
        protected override void OnSleep()
        {
        }
        protected override void OnResume()
        {
        }
    }
}