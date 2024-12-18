using SimpleCrm.Models;

namespace SimpleCrm.Specification
{
    public class ClientsToExportSpec:BaseSpecification<Clients>
    {
        public ClientsToExportSpec():base()
        {
            Includes.Add(z => z.CommingReason);
        }
    }
}
