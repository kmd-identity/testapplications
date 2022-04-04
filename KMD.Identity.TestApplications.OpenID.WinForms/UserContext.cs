using Microsoft.Identity.Client;

namespace KMD.Identity.TestApplications.OpenID.WinForms
{
    /// <summary>
    /// This class serves only example purpose.
    /// In real app it should be designed better.
    /// </summary>
    public class UserContext
    {
        private static UserContext userContext;

        public AuthenticationResult AuthenticatedUser { get; private set; }

        public static UserContext FromResult(AuthenticationResult result)
        {
            userContext = new UserContext { AuthenticatedUser = result };
            return userContext;
        }

        public static UserContext Current => userContext;
    }
}
