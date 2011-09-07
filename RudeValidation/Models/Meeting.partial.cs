using System;

namespace RudeValidation.Web.Models
{
    public partial class Meeting
    {
        partial void OnCreated()
        {
            this.Start = (this.End = DateTime.Today.AddDays(1));
        }
    }
}
