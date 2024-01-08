using System.Collections.Generic;
using KMD.Identity.TestApplications.OpenID.API.Models;

namespace KMD.Identity.TestApplications.OpenID.API.Services.Financial
{
    public interface IFinancialService
    {
        OperationResult<List<decimal>> Get(string subject);

        OperationResult<List<decimal>> Pay(string subject);

        //technical method
        void CleanupAll();

        //technical method
        void Cleanup(string subject);
    }
}