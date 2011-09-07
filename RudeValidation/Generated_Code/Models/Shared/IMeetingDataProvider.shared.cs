using System.Collections.Generic;

namespace RudeValidation.Web.Models
{
    public interface IMeetingDataProvider
    {
        IEnumerable<Meeting> Meetings { get; }
        IEnumerable<Location> Locations { get; }
    }
}
