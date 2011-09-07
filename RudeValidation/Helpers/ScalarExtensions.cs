using System.Linq;

namespace RudeValidation.Helpers
{
    public static class ScalarExtensions
    {
        public static bool In(this string value, params string[] list)
        {
            return list.Contains(value);
        }
    }
}
