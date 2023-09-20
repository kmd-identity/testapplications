using Android.App;
using Android.OS;

namespace KMD.Identity.TestApplications.OpenID.MAUI;

/// <summary>
/// This Activity is used to handle Logout response from Identity Provider.
/// It must be registered in AndroidManifest.xml with proper data:scheme and data:host.
/// </summary>
[Activity(Exported = true, Name = "kmd.identity.testapplication.maui.PostLogoutActivity")]
public class PostLogoutActivity : Activity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        
        SetResult(Result.Canceled);
        Finish();
    }
}