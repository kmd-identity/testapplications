using Microsoft.AspNetCore.Mvc;

namespace KMD.Identity.TestApplications.MainPage.Controllers
{
    public class MobileRegistrationsController : Controller
    {
        /// <summary>
        /// Android AppLinks support for MAUI application.
        /// </summary>
        /// <returns></returns>
        [Route("/.well-known/assetlinks.json")]
        public IActionResult WellKnownAssetLinks()
        {
            return Content(@"
[
  {
    ""relation"": [ ""delegate_permission/common.handle_all_urls"" ],
    ""target"": {
      ""namespace"": ""android_app"",
      ""package_name"": ""dk.kmd.identity.testapplication"",
      ""sha256_cert_fingerprints"": [ ""DB:50:EB:53:82:4F:45:62:EA:57:55:A6:5C:B9:6C:40:9F:9C:3A:0C:A3:EF:FB:26:34:54:7C:CF:51:C4:33:CD"" ]
    }
  }
]", "application/json");
        }
    }
}