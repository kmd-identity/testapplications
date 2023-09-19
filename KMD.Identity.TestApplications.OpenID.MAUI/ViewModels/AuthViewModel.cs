using Microsoft.Identity.Client;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Environment = System.Environment;

namespace KMD.Identity.TestApplications.OpenID.MAUI.ViewModels;

public class AuthViewModel : INotifyPropertyChanged
{
    private bool isAuthenticated;
    private string claims;

    public bool IsAuthenticated
    {
        get => isAuthenticated;
        set => SetField(ref isAuthenticated, value);
    }

    public string Claims
    {
        get => claims;
        set => SetField(ref claims, value);
    }

    public event PropertyChangedEventHandler PropertyChanged;

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

    public void AfterAuthentication(AuthenticationResult result)
    {
        IsAuthenticated = result?.ClaimsPrincipal?.Claims?.Any() == true;

        if (!IsAuthenticated)
        {
            Claims = string.Empty;
            return;
        }

        Claims = string.Join(Environment.NewLine, result.ClaimsPrincipal.Claims.Select(c => $"{c.Type}: {c.Value}"));
    }

    public void AfterLogout()
    {
        IsAuthenticated = false;
        Claims = string.Empty;
    }
}