namespace SimpleCrm.Models
{
    public class Project
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Objectives { get; set; }
        public string ProjectManger { get; set; }
        public bool Ended { get; set; }
        public DateTime StartedAt { get; set; }
    }
}
