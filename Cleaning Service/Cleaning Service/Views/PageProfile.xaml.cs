using Cleaning_Service.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Cleaning_Service.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageProfile : ContentPage
    {
        public PageProfile()
        {
            InitializeComponent();
            BindingContext = new ProfileModelView();
        }
    }
}