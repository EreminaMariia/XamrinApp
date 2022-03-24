using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CalendarApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new Page();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
