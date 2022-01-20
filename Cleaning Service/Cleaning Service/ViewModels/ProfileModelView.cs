using Cleaning_Service.Models;
using Plugin.CloudFirestore;
using Plugin.FirebaseAuth;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Cleaning_Service.ViewModels
{
    public class ProfileModelView: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string firstName;
        private string lastName;
        private string email;
        private string phone;
        private string address;
        private string role;
        public string FirstName { get { return firstName; } set { firstName = value; PropertyChanged(this, new PropertyChangedEventArgs("FirstName")); } }
        public string LastName { get { return lastName; } set { lastName = value; PropertyChanged(this, new PropertyChangedEventArgs("LastName")); } }
        public string Email { get { return email; } set { email = value; PropertyChanged(this,  new PropertyChangedEventArgs("Email")); } }
        public string Phone { get { return phone; } set { phone = value; PropertyChanged(this,  new PropertyChangedEventArgs("Phone")); } }
        public string Address { get { return address; } set { address = value; PropertyChanged(this, new PropertyChangedEventArgs("Address")); } }
        public string Role { get { return role; } set { role = value; PropertyChanged(this, new PropertyChangedEventArgs("Role")); } }
        public ICommand OnUpdateCommand { get; set; }   
        public ProfileModelView()
        {
            OnUpdateCommand = new Command(Update);

            GetData();
        }

        private void GetData()
        {
            CrossCloudFirestore
                .Current
                .Instance
                .Collection("USERS")
                .Document(CrossFirebaseAuth.Current.Instance.CurrentUser.Uid)
                .AddSnapshotListener((value, error) =>
                {
                    if (value.Exists)
                    {
                        var user = value.ToObject<User>();
                        FirstName = user.FirstName;
                        LastName = user.LastName;
                        Email = user.Email;
                        Phone = user.PhoneNumber;
                        Address = user.Address;
                        Role = user.Role;
                    }
                });
        }

        private void Update()
        {


            ///**************////
            ///

            //validattion

            if(firstName == null)
            {
                App.Current.MainPage.DisplayAlert("Warning", "Enter first name", "Ok");
                return;
            }

            if(lastName == null)
            {
                App.Current.MainPage.DisplayAlert("Warning", "Enter last name", "Ok");
                return;
            }

            if(email == null)
            {
                App.Current.MainPage.DisplayAlert("Warning", "Enter email ", "Ok");
                return;
            }

            if(phone == null)
            {
                App.Current.MainPage.DisplayAlert("Warning", "Enter phone", "Ok");
                return;
            }

            if(address == null)
            {
                App.Current.MainPage.DisplayAlert("Warning", "Enter address", "Ok");
                return;
            }

            if(role == null)
            {
                App.Current.MainPage.DisplayAlert("Warning", "Enter gender", "Ok");
                return;
            }


            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"FirstName", firstName},
                {"Email", email},
                {"Address", address},
                {"Role", role},
                {"PhoneNumber", phone},
                {"LastName", lastName},
            };
            CrossCloudFirestore
                .Current
                .Instance
                .Collection("Users")
                .Document(CrossFirebaseAuth.Current.Instance.CurrentUser.Uid)
                .UpdateAsync(data);
        }
    }
}
