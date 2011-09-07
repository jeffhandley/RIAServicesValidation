namespace RudeValidation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.ServiceModel.DomainServices.Client;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Navigation;
    using RudeValidation.Helpers;
    using RudeValidation.Web.Models;
    using RudeValidation.Web.Services;

    /// <summary>
    /// Home page for the application.
    /// </summary>
    public partial class Home : Page
    {
        MeetingContext meetingContext = new MeetingContext();

        /// <summary>
        /// Creates a new <see cref="Home"/> instance.
        /// </summary>
        public Home()
        {
            InitializeComponent();

            this.Title = ApplicationStrings.HomePageTitle;
        }

        /// <summary>
        /// Executes when the user navigates to this page.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.meetingContext.Load(this.meetingContext.GetLocationsQuery().Where(l => l.LocationName.StartsWith("18/")));

            Meeting meeting = new Meeting
            {
                Title = "Waste Your Day!",
                Start = DateTime.Today.AddDays(2).AddHours(14),
                End = DateTime.Today.AddDays(2).AddHours(15),
                Location = "18/3002",
                MinimumAttendees = 5,
                MaximumAttendees = 10,
                Details = "This meeting will cost the company a fortune!"
            };

            var contextItems = new Dictionary<object, object>
            {
                { "AllowOverBooking", this.allowOverBooking.IsChecked ?? false }
            };

            var contextServiceProvider = new SimpleServiceProvider();
            contextServiceProvider.AddService<IMeetingDataProvider>(this.meetingContext);

            this.meetingContext.ValidationContext = new ValidationContext(
                this.meetingContext, contextServiceProvider, contextItems);

            this.meetingContext.Meetings.Add(meeting);
            ((IEditableObject)meeting).BeginEdit();

            this.LayoutRoot.DataContext = meeting;

            this.meetingDataGrid.ItemsSource = new List<Meeting> { meeting };
            this.meetingDataForm.CurrentItem = meeting;
        }

        private void Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            IEditableObject meeting = (Meeting)this.LayoutRoot.DataContext;
            meeting.EndEdit();

            this.meetingContext.SubmitChanges(op =>
                {
                    if (op.HasError)
                    {
                        op.MarkErrorAsHandled();

                        if (op.EntitiesInError.Any())
                        {
                            this.EntityHeader.Focus();
                        }
                    }
                }, null);
        }

        private void meetingDataForm_EditEnded(object sender, DataFormEditEndedEventArgs e)
        {
            if (e.EditAction == DataFormEditAction.Commit)
            {
                Action<SubmitOperation> completed = operation =>
                {
                    if (operation.HasError && operation.EntitiesInError.Any(entity => entity.HasValidationErrors))
                    {
                        operation.MarkErrorAsHandled();
                    }
                };

                this.meetingContext.SubmitChanges(completed, null);
            }
        }

        private void meetingDataForm_AutoGeneratingField(object sender, DataFormAutoGeneratingFieldEventArgs e)
        {
            if (e.PropertyName.In("Start", "End"))
            {
                e.Field.ReplaceDatePicker(new TextBox(), TextBox.TextProperty);
            }
        }

        private void allowOverBooking_Click(object sender, RoutedEventArgs e)
        {
            this.meetingContext.ValidationContext.Items["AllowOverBooking"] = this.allowOverBooking.IsChecked ?? false;
            this.meetingContext.SetOverBookingSetting(this.allowOverBooking.IsChecked ?? false);
        }
    }
}