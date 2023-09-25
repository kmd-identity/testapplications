using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KMD.Identity.TestApplications.OpenID.MAUI.ViewModels
{
    public class ApiPageViewModel: INotifyPropertyChanged
    {
        private string apiResult;
        public event PropertyChangedEventHandler PropertyChanged;

        public string ApiResult
        {
            get => apiResult;
            set => SetField(ref apiResult, value);
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
