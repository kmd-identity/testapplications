using Microsoft.Identity.Client;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using Environment = System.Environment;

namespace KMD.Identity.TestApplications.OpenID.MAUI.ViewModels;

public class AuthViewModel : INotifyPropertyChanged
{
    private bool isAuthenticated;
    private Claim[] claims;

    public bool IsAuthenticated
    {
        get => isAuthenticated;
        set => SetField(ref isAuthenticated, value);
    }

    public Claim[] Claims
    {
        get => claims;
        set => SetField(ref claims, value);
    }

    public string AccessToken { get; set; }
    public string IdToken { get; set; }

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
            Claims = Array.Empty<Claim>();
            return;
        }

        Claims = result!.ClaimsPrincipal!.Claims.ToArray();
        AccessToken = result.AccessToken;
        IdToken = result.IdToken;
    }

    public void AfterLogout()
    {
        IsAuthenticated = false;
        Claims = Array.Empty<Claim>();
        AccessToken = string.Empty;
        IdToken = string.Empty;
    }
}