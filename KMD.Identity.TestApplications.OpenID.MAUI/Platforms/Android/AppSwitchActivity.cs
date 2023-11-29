using Android.App;
using Android.OS;

namespace KMD.Identity.TestApplications.OpenID.MAUI;

[Activity(Exported = true, Name = "kmd.identity.testapplication.maui.AppSwitchActivity")]
public class AppSwitchActivity : Activity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        
        SetResult(Result.Canceled);
        Finish();
    }
}