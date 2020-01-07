using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CourseLibrary.API.Models
{
    public class AuthorForCreationDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTimeOffset DateOfBirth { get; set; }

        public string MainCategory { get; set; }
        public ICollection<CourseForCreationDto> Courses { get; set; }
                                = new Collection<CourseForCreationDto>();
    }
}