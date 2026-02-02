using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Identity.Client;

namespace KMD.Identity.TestApplications.OpenID.MAUI.ViewModels
{
    public class LoginViewModel: INotifyPropertyChanged
    {
        private bool hasBiometric;
        private bool hasValidToken;
        public event PropertyChangedEventHandler PropertyChanged;

        public string DomainHint { get; set; }
        public string LoginHint { get; set; }

        public AuthenticationResult AuthenticationResult { get; set; }

        public bool HasValidToken
        {
            get => hasValidToken;
            set => SetField(ref hasValidToken, value);
        }

        public bool HasBiometric
        {
            get => hasBiometric;
            set => SetField(ref hasBiometric, value);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
