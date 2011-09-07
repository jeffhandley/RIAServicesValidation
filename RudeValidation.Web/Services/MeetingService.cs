
namespace RudeValidation.Web.Services
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data;
    using System.Linq;
    using System.ServiceModel.DomainServices.EntityFramework;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using System.Web;
    using RudeValidation.Web.Models;


    // Implements application logic using the MeetingStoreEntities context.
    // TODO: Add your application logic to these methods or in additional methods.
    // TODO: Wire up authentication (Windows/ASP.NET Forms) and uncomment the following to disable anonymous access
    // Also consider adding roles to restrict access as appropriate.
    // [RequiresAuthentication]
    [EnableClientAccess()]
    public class MeetingService : LinqToEntitiesDomainService<MeetingStoreEntities>, IMeetingDataProvider
    {
        /// <summary>
        /// Initialize the <see cref="DomainService"/>, setting the
        /// <see cref="ValidationContext"/> with custom state and services.
        /// </summary>
        /// <param name="context">
        /// Represents the execution environment for the operations performed
        /// by a System.ServiceModel.DomainServices.Server.DomainService.
        /// </param>
        public override void Initialize(DomainServiceContext context)
        {
            var contextItems = new Dictionary<object, object>
            {
                { "AllowOverBooking", HttpContext.Current.Session["AllowOverBooking"] ?? false }
            };

            this.ValidationContext = new ValidationContext(this, context, contextItems);
            this.ValidationContext.ServiceContainer.AddService(typeof(IMeetingDataProvider), this);

            base.Initialize(context);
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'Locations' query.
        public IQueryable<Location> GetLocations()
        {
            return this.ObjectContext.Locations;
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'Meetings' query.
        public IQueryable<Meeting> GetMeetings()
        {
            return this.ObjectContext.Meetings;
        }

        public void InsertMeeting(Meeting meeting)
        {
            if ((meeting.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(meeting, EntityState.Added);
            }
            else
            {
                this.ObjectContext.Meetings.AddObject(meeting);
            }
        }

        public void UpdateMeeting(Meeting currentMeeting)
        {
            this.ObjectContext.Meetings.AttachAsModified(currentMeeting, this.ChangeSet.GetOriginal(currentMeeting));
        }

        public void DeleteMeeting(Meeting meeting)
        {
            if ((meeting.EntityState == EntityState.Detached))
            {
                this.ObjectContext.Meetings.Attach(meeting);
            }
            this.ObjectContext.Meetings.DeleteObject(meeting);
        }

        [Update(UsingCustomMethod = true)]
        public void CancelMeeting(Meeting meeting, [Required] string reason)
        {

        }

        [Invoke]
        public int GetMeetingCount([Required] string attendeeName)
        {
            return 0;
        }

        public IEnumerable<Meeting> GetMeetingsForAttendee([Required] string attendeeName)
        {
            return Enumerable.Empty<Meeting>();
        }

        public IEnumerable<Meeting> Meetings
        {
            get { return this.ObjectContext.Meetings; }
        }

        public IEnumerable<Location> Locations
        {
            get { return this.ObjectContext.Locations; }
        }

        [Invoke]
        public void SetOverBookingSetting(bool allowOverBooking)
        {
            HttpContext.Current.Session["AllowOverBooking"] = allowOverBooking;
        }
    }
}


