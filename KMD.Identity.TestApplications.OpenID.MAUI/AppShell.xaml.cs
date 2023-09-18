namespace KMD.Identity.TestApplications.OpenID.MAUI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            BindingContext = App.ViewModel;
        }
    }
}