using Cleaning_Service.MaidView;
using Plugin.CloudFirestore;
using Plugin.FirebaseAuth;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace Cleaning_Service.ViewModels
{
    public class MaidHomeViewModel : INotifyPropertyChanged
    {
        //private string id;
        //private string uid;
        //private string mid;
        //private string user;
        //private string lat;
        //private string lon;
        //private string address;
        //private string price;
        //private string startDate;
        //private string status;

        //public string Id { get { return id; } set { id = value; PropertyChanged(this, new PropertyChangedEventArgs("Id")); } }
        //public string User { get { return user; }set { user = value; PropertyChanged(this,new PropertyChangedEventArgs("User")); } }
        //public string Lat { get { return lat; } set { lat = value; PropertyChanged(this, new PropertyChangedEventArgs("Lat")); } }
        //public string Lon { get { return lon; } set { lon = value; PropertyChanged(this, new PropertyChangedEventArgs("Lon")); } }
        //public string Address { get { return address; } set { address = value; PropertyChanged(this, new PropertyChangedEventArgs("Address")); } }
        //public string Price { get { return price; }set { price = value; PropertyChanged(this, new PropertyChangedEventArgs("Price")); } }
        //public string StartDate { get { return startDate; }set { startDate = value; PropertyChanged(this, new PropertyChangedEventArgs("StartDate")); } }
        //public string Status { get { return status; }set { status = value; PropertyChanged(this, new PropertyChangedEventArgs("Status")); } }
        //public string Uid { get { return uid; }set { uid = value; PropertyChanged(this, new PropertyChangedEventArgs("Uid")); } }
        //public string Mid { get { return mid; }set { mid = value; PropertyChanged(this, new PropertyChangedEventArgs("Mid")); } }
        private ObservableCollection<Requests> requests = new ObservableCollection<Requests>();
        public ObservableCollection<Requests> Requests { get { return requests; } }

        //private void  UpdateValues(string id, string uid, string mid, string user, string lat, string lon, string address, string price, string startDate, string status)
        //{
        //    this.id = id;
        //    this.uid = uid;
        //    this.mid = mid;
        //    this.user = user;
        //    this.lat = lat;
        //    this.lon = lon;
        //    this.address = address;
        //    this.price = price;
        //    this.startDate = startDate;
        //    this.status = status;
        //}

        public MaidHomeViewModel()
        {
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
                            data.User = await GetUserAsync(data.Uid);
                            //UpdateValues(data.Id, data.Uid, data.Mid, data.Uid, data.Lat, data.Lon, data.Address, data.Price, data.StartDate, data.Status);
                            switch (item.Type)
                            {
                                case DocumentChangeType.Added:
                                    requests.Add(data);
                                    break;
                                case DocumentChangeType.Modified:

                                    for (int i = 0; i < requests.Count; i++)
                                    {
                                        if (requests[i].Id == data.Id)
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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
