using System;

namespace RudeValidation.Web.Models
{
    public interface IStartAndEnd
    {
        DateTime Start { get; }
        DateTime End { get; }
    }
}
