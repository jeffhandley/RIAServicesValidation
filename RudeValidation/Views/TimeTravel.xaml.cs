namespace RudeValidation
{
    using System.Windows.Controls;
    using System.Windows.Navigation;

    /// <summary>
    /// <see cref="Page"/> class to present information about the current application.
    /// </summary>
    public partial class TimeTravel : Page
    {
        /// <summary>
        /// Creates a new instance of the <see cref="TimeTravel"/> class.
        /// </summary>
        public TimeTravel()
        {
            InitializeComponent();

            this.Title = "Time travel is not allowed";
        }

        /// <summary>
        /// Executes when the user navigates to this page.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}