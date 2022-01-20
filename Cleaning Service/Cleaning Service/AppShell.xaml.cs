using Cleaning_Service.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Cleaning_Service
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage)); 
            
        }

    }
}

