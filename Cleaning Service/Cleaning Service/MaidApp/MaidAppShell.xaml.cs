using Cleaning_Service.MaidView;
using Cleaning_Service.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Cleaning_Service.MaidApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MaidAppShell : Xamarin.Forms.Shell
    {
        public MaidAppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(MaidHomePage), typeof(MaidHomePage));
        }
    }
}