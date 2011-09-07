using System;
using System.Collections.Generic;
using RudeValidation.Helpers;
using RudeValidation.Web.Models;

namespace RudeValidation.ViewModels
{
    [ModelObjectValidator(typeof(Meeting))]
    public class MeetingEntryViewModel : ViewModelBase, IStartAndEnd
    {
        private DateTime start;
        private DateTime end;
        private bool isAllDay;
        private string title;
        private string details;
        private string location;
        private int minAttendees;
        private int maxAttendees;
        private TimeSpan reminder;
        private List<string> invitees;

        public MeetingEntryViewModel()
        {
            this.Start = DateTime.Today.AddHours(5);
        }

        [ModelPropertyValidator(typeof(Meeting))]
        public DateTime Start
        {
            get { return this.start; }
            set
            {
                if (this.start != value)
                {
                    this.ValidateProperty("Start", value);
                    this.start = value;
                    this.RaisePropertyChanged("Start");
                }
            }
        }

        [ModelPropertyValidator(typeof(Meeting))]
        public DateTime End
        {
            get { return this.end; }
            set
            {
                if (this.end != value)
                {
                    this.ValidateProperty("End", value);
                    this.end = value;
                    this.RaisePropertyChanged("End");
                }
            }
        }
    }
}
