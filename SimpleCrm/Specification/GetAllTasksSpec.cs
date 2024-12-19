
namespace SimpleCrm.Specification
{
    public class GetAllTasksSpec:BaseSpecification<Models.Tasks>
    {
        public GetAllTasksSpec():base()
        {
            Includes.Add(z=>z.User);
            OrderBy = z => z.EndAt;
        }
    }
}
