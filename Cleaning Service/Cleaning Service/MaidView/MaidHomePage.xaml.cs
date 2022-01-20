using Cleaning_Service.Models;
using Cleaning_Service.ViewModels;
using MimeKit;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;
using Plugin.FirebaseAuth;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Cleaning_Service.MaidView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MaidHomePage : ContentPage
    {
        public MaidHomePage()
        {
            InitializeComponent();

            LoadRequests();
        }
        private readonly ObservableCollection<Requests> requests = new ObservableCollection<Requests>();
        public ObservableCollection<Requests> Requests { get { return requests; } }
        private void LoadRequests()
        {
            //System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");
            //cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
            //System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
            RequestedMaidService.ItemsSource = Requests;
            CrossCloudFirestore
                .Current
                .Instance
                .Collection("REQUESTS")
                .WhereEqualsTo("Mid", CrossFirebaseAuth.Current.Instance.CurrentUser.Uid)
                .AddSnapshotListener(async (v, e) =>
                {
                    if (!v.IsEmpty)
                    {
                        foreach (var item in v.DocumentChanges)
                        {
                            var data = item.Document.ToObject<Requests>();

                            switch (item.Type)
                            {
                                case DocumentChangeType.Added:
                                    data.User = await GetUserAsync(data.Uid);
                                    data.BtnVisible = true;
                                    if (data.Status == "Accepted")
                                    {
                                        // IsVisible = false;
                                        data.BtnVisible = false;

                                    }
                                    requests.Add(data);
                                    break;
                                case DocumentChangeType.Modified:

                                    for (int i = 0; i < requests.Count; i++)
                                    {
                                        if (requests[i].Id == data.Id)
                                        {
                                            data.BtnVisible = true;
                                            if (data.Status != "Accepted")
                                            {
                                                requests[i] = data;
                                                //  IsVisible = false;
                                                data.BtnVisible = false;
                                                var query = await CrossCloudFirestore
                                                    .Current
                                                    .Instance
                                                    .Collection("USERS")
                                                    .Document(Plugin.FirebaseAuth.CrossFirebaseAuth.Current.Instance.CurrentUser.Uid)
                                                    .GetAsync();
                                                if (query != null)
                                                {
                                                    var user = query.ToObject<User>();
                                                    var to = await GetUserEmailAsync(data.Uid);
                                                    SendEmailAsync(user, to);
                                                }

                                                break;
                                            }
                                            else if (data.Status != "Rejected")
                                            {

                                                requests.RemoveAt(i);
                                                break;
                                            }
                                        }
                                    }
                                    break;
                                case DocumentChangeType.Removed:
                                    break;
                            }
                        }
                    }
                });
        }
        private async Task<string> GetUserAsync(string uid)
        {
            try
            {
                if(uid == null)
                {
                    return null;
                }
                var query = await CrossCloudFirestore
                    .Current
                    .Instance
                    .Collection("USERS")
                    .Document(uid)
                    .GetAsync();
                if (query != null)
                {
                    var user = query.ToObject<User>();
                    return $"{user.FirstName} {user.LastName}";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }
        private async Task<string> GetUserEmailAsync(string uid)
        {
            try
            {
                var query = await CrossCloudFirestore
                    .Current
                    .Instance
                    .Collection("USERS")
                    .Document(uid)
                    .GetAsync();
                if (query != null)
                {
                    var user = query.ToObject<User>();
                    return user.Email;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }
        private void StatusCheck_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value == true)
            {
                UpdateStatus("1");
            }
            else
            {
                UpdateStatus("0");
            }
        }
        private static void UpdateStatus(string value)
        {
            CrossCloudFirestore
               .Current
               .Instance
               .Collection("USERS")
               .Document(CrossFirebaseAuth.Current.Instance.CurrentUser.Uid)
               .UpdateAsync("Status", value);
        }

        private void BtnLogout_Clicked(object sender, EventArgs e)
        {
            CrossFirebaseAuth.Current.Instance.SignOut();
        }

        private async void BtnAccept_Clicked(object sender, EventArgs e)
        {
            var data = (Button)sender;
            var id = data.CommandParameter.ToString();
            await CrossCloudFirestore
                .Current
                .Instance
                .Collection("REQUESTS")
                .Document(id)
                .UpdateAsync("Status", "Accepted");


            

        }

        private void BtnReject_Clicked(object sender, EventArgs e)
        {
            var data = (Button)sender;
            var id = data.CommandParameter.ToString();
            CrossCloudFirestore
                .Current
                .Instance
                .Collection("REQUESTS")
                .Document(id)
                .UpdateAsync("Status", "Rejected");
        }

        private void BtnCall_Clicked(object sender, EventArgs e)
        {
            var data = (Button)sender;
            var id = data.CommandParameter.ToString();
        }

        private async void BtnNavigate_Clicked(object sender, EventArgs e)
        {
            var data = (Button)sender;
            var id = data.CommandParameter.ToString();

            var query = await CrossCloudFirestore
                    .Current
                    .Instance
                    .Collection("USERS")
                    .Document(id)
                    .GetAsync();
            if (query != null)
            {
                var user = query.ToObject<User>();
                Xamarin.Essentials.PhoneDialer.Open(user.PhoneNumber);
            }
        }

        private async void SendEmailAsync(User user, string to)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("RENT A MAID", "sigauquetk@gmail.com"));
            message.To.Add(new MailboxAddress($"Message", to.Trim()));
            message.Subject = "Service Accepted";
            string body = $"{user.FirstName} {user.LastName} Has Accepted your request \n Phone Number {user.PhoneNumber}, email {user.Email}";


            message.Body = new TextPart("html")
            {
                Text = body,

                //Text = $"Book title: {Items[e.Position].Title}" +
                //$" Download Url: {Items[e.Position].ImageUrl}",
            };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {

                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect("smtp.gmail.com", 587);
                client.Authenticate("sigauquetk@gmail.com", "THIma$!305");
                await client.SendAsync(message);
            };
        }


    }
    public class Requests
    {
        [Id]
        public string Id { get; set; }
        public string Uid { get; set; }
        public string Mid { get; set; }
        public string User { get; set; }
        public string Lat { get; set; }
        public string Lon { get; set; }
        public string Address { get; set; }
        public string Price { get; set; }
        public string StartDate { get; set; }
        public string Status { get; set; }
        public bool BtnVisible { get; set; }
    }
    public class User2
    {
        [Id]
        public string Id { get; set; }
        [MapTo("FirstName")]
        public string FirstName { get; set; }
        [MapTo("LastName")]
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Role { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Status { get; set; }
        

    }
}