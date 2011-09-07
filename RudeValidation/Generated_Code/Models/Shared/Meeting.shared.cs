using System;
using System.ComponentModel.DataAnnotations;

namespace RudeValidation.Web.Models
{
    public partial class Meeting : IStartAndEnd
    {
        [Display(AutoGenerateField = false)]
        public int BuildingNumber
        {
            get { return int.Parse(this.Location.Substring(0, this.Location.IndexOf("/"))); }
        }

        [Display(AutoGenerateField = false)]
        public int RoomNumber
        {
            get { return int.Parse(this.Location.Substring(this.Location.IndexOf("/") + 1)); }
        }

        [Display(AutoGenerateField = false)]
        public bool IsLongMeeting
        {
            get { return this.End.Subtract(this.Start) > new TimeSpan(1, 0, 0); }
        }
    }
}