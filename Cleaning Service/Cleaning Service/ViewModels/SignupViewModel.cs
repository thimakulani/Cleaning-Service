using Cleaning_Service.Models;
using Cleaning_Service.Views;
using Plugin.CloudFirestore;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Cleaning_Service.ViewModels
{
    public class SignupViewModel:INotifyPropertyChanged
    {
        private string name;
        private string lastname;
        private string password;
        private string phoneNumber; 
        private string email;
        private string address;
        private string role;
        private string Latitude;
        private string Longitude; 
        public event PropertyChangedEventHandler PropertyChanged;
        public string Name { get { return name; } set { name = value; PropertyChanged(this, new PropertyChangedEventArgs("Name")); } }
        public string LastName { get { return lastname; } set { lastname = value; PropertyChanged(this, new PropertyChangedEventArgs("LastName")); } }
        public string Password { get { return password; }set { password = value; PropertyChanged(this,new PropertyChangedEventArgs("Password")); } }
        public string PhoneNumber { get { return phoneNumber; } set { phoneNumber = value; PropertyChanged(this, new PropertyChangedEventArgs("PhoneNumber")); } }
        public string Email { get { return email; } set { email = value; PropertyChanged(this, new PropertyChangedEventArgs("Email")); } }
        public string Address { get { return address; } set { address = value; PropertyChanged(this,new PropertyChangedEventArgs ("Address")); } }
        public string Role { get { return role; } set { role = value; PropertyChanged(this, new PropertyChangedEventArgs("Role")); } }
        
        public ICommand OnSignUpCommand { get; set; }
        public ICommand OnSubmitLocation { get; set; }
        public ICommand RadioClick { get; set; }
        private async void Signup()
        {

            if (Email == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Enter email", "Got it");
                return;
            }
            if (Password == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Enter Password", "Got it");
                return;
            }
            if (Address == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Select Address", "Got it");
                return;
            }
            if(Role == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Select Role", "Got it");
                return;
            }
            try
            {
                User user = new User()
                {
                    Email = Email,
                    PhoneNumber = PhoneNumber,
                    Address = Address,
                    FirstName = Name,
                    LastName = LastName,
                    Latitude = Latitude,
                    Longitude = Longitude,
                    Role = Role,
                    Status = null,
                };
                var results = await Plugin.FirebaseAuth.CrossFirebaseAuth
                    .Current
                    .Instance
                    .CreateUserWithEmailAndPasswordAsync(Email, Password);

                user.Id = results.User.Uid;
                await CrossCloudFirestore
                    .Current
                    .Instance
                    .Collection("USERS")
                    .Document(results.User.Uid)
                    .SetAsync(user);
            }
            catch (Plugin.FirebaseAuth.FirebaseAuthException ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Okay");
            }
        }

        public SignupViewModel()
        {
            OnSignUpCommand = new Command(Signup);
            OnSubmitLocation = new Command(UpdateAddress);
            RadioClick = new Command(RadioSelecte);
        }

        private void RadioSelecte(object obj)
        {
            var item = (RadioButton)obj;
            App.Current.MainPage.DisplayAlert("Error", item.Value.ToString(), "Okay");
        }

        private async void UpdateAddress(object obj)
        {
            var map = new MapDialog();
            map.AddressChanged += Map_AddressChanged;
            await PopupNavigation.Instance.PushAsync(map);
        }

        private void Map_AddressChanged(object sender, MapDialog.AddressEventHandler e)
        {
            Address = e.Address;
            Latitude = e.Latitude;
            Longitude = e.Longitude;    

        }
    }
    
}
 