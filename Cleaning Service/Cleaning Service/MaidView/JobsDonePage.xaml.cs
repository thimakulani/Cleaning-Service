using Cleaning_Service.Models;
using Plugin.CloudFirestore;
using Plugin.FirebaseAuth;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Cleaning_Service.MaidView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class JobsDonePage : ContentPage
    {
        private readonly ObservableCollection<Requests> requests = new ObservableCollection<Requests>();
        public ObservableCollection<Requests> Requests { get { return requests; } }
        public JobsDonePage()
        {
            InitializeComponent();
            LoadRequests();
        }
        private void LoadRequests()
        {
            //System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");
            //cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
            //System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
            RequestsHistory.ItemsSource = Requests;
            CrossCloudFirestore
                .Current
                .Instance
                .Collection("REQUESTS")
                .WhereEqualsTo("Mid", CrossFirebaseAuth.Current.Instance.CurrentUser.Uid)
                .WhereIn("Status",new object[] {"Accepted", "Rejected" } )
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
                if (uid == null)
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
    }
}