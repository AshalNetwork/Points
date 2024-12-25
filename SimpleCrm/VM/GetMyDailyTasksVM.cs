namespace SimpleCrm.VM
{
    public class GetMyDailyTasksVM
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string CompletedBy { get; set; }
    }
}
