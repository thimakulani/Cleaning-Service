using Cleaning_Service.MaidView;
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

namespace Cleaning_Service.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecentRequestsPage : ContentPage
    {
        private ObservableCollection<Requests> requests = new ObservableCollection<Requests>();
        public ObservableCollection<Requests> Requests { get { return requests; } }
        public RecentRequestsPage()
        {
            InitializeComponent();
            RequestedMaids.ItemsSource = Requests;
            LoadRequests();
        }
       private void LoadRequests()
        {
            CrossCloudFirestore
                .Current
                .Instance
                .Collection("REQUESTS")
                .WhereEqualsTo("Uid", CrossFirebaseAuth.Current.Instance.CurrentUser.Uid)
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
                                    data.User = await GetUserAsync(data.Mid);
                                    requests.Add(data);
                                    break;
                                case DocumentChangeType.Modified:

                                    for (int i = 0; i < requests.Count; i++)
                                    {
                                        if(requests[i].Id == data.Id)
                                        {
                                            requests[i] = data;
                                            break;
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
                var query = await CrossCloudFirestore
                    .Current
                    .Instance
                    .Collection("USERS")
                    .Document(uid)
                    .GetAsync();
                if (query != null)
                {
                    var user = query.ToObject<User2>();
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