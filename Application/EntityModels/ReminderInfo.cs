﻿using Domain.Enums;

namespace Application.EntityModels
{
    public class ReminderInfo
    {
        public int ReminderId { get; set; }
        public string Title { get; set; }
        public DateTime DateTime { get; set; }
        public PriorityLevel PriorityLevel { get; set; }
        public string? Notes { get; set; }
        public ReminderStatus Status { get; set; }
        public int DaysBeforeToRemind { get; set; }
        public bool RemindByEmail { get; set; }

        public ReminderInfo(int reminderId, string title, DateTime dateTime,
                         PriorityLevel priority, string notes, ReminderStatus status, int dToRemind, bool rBy)
        {
            this.ReminderId = reminderId;
            this.Title = title;
            this.DateTime = dateTime;
            this.PriorityLevel = priority;
            this.Notes = notes;
            this.Status = status;
            this.DaysBeforeToRemind = dToRemind;
            this.RemindByEmail = rBy;
        }
    }
}
