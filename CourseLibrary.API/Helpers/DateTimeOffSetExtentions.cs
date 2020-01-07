using System;

namespace CourseLibrary.API.Helpers
{
    public static class DateTimeOffSetExtentions
    {
        public static int GetCurrentAge(this DateTimeOffset date)
        {
            var currentDate = DateTimeOffset.UtcNow;
            int age = currentDate.Year - date.Year;

            if (currentDate < date.AddYears(age))
                age--;

            return age;
        }
    }
}