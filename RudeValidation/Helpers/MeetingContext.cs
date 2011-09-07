using System.Collections.Generic;
using RudeValidation.Web.Models;

namespace RudeValidation.Web.Services
{
    public sealed partial class MeetingContext : IMeetingDataProvider
    {
        IEnumerable<Meeting> IMeetingDataProvider.Meetings
        {
            get { return this.Meetings; }
        }

        IEnumerable<Location> IMeetingDataProvider.Locations
        {
            get { return this.Locations; }
        }
    }
}
