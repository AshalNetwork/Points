﻿namespace SimpleCrm.VM
{
    public class AllTasksVM
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string User { get; set; }
        public string UserId { get; set; }
        public string Role { get; set; }
    }
}