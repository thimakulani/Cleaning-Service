using Cleaning_Service.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;

namespace Cleaning_Service.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string email;
        private string password;

        public string Email { get { return email; }set { email = value; PropertyChanged(this, new PropertyChangedEventArgs("Email")); } }
        public string Password { get { return password; }set { password = value; PropertyChanged(this,new PropertyChangedEventArgs("Password")); } }

        public ICommand SubmitCommand { get; set; }
        public ICommand OnSignUpNavigation { get; set; }
        public ICommand OnForgotPassword { get; set; }  
        public LoginViewModel()
        {
            SubmitCommand = new Command(Login);
            OnSignUpNavigation = new Command(SignUp);
            OnForgotPassword = new Command(ForgotPassword);
        }
        private void ForgotPassword()
        {

        }
        private void SignUp()
        {
            App.Current.MainPage.Navigation.PushModalAsync(new RegisterPage());
        }

        private async void Login()
        {
            
            try
            {
                if (Email == null)
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Enter email", "Ok");
                    return;
                }
                if (Password == null)
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Enter Password", "Ok");
                    return;
                }
                await Plugin.FirebaseAuth.CrossFirebaseAuth.Current
                    .Instance
                    .SignInWithEmailAndPasswordAsync(Email, Password);


            }
            catch (Plugin.FirebaseAuth.FirebaseAuthException ex)
            {
               await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Ok");
            }
            
        }
    }
}
