using System;
using System.Collections.Generic;
using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("api/authors/{authorId}/courses")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;

        public CoursesController(ICourseLibraryRepository courseLibraryRepository,
                                IMapper mapper)
        {
            _courseLibraryRepository = courseLibraryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CourseDto>> GetCoursesForAuthor(Guid authorId)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
                return NotFound();

            var courses = _courseLibraryRepository.GetCourses(authorId);

            return Ok(_mapper.Map<IEnumerable<CourseDto>>(courses));
        }

        [HttpGet("{courseId}", Name = nameof(GetCourseForAuthor))]
        public ActionResult<CourseDto> GetCourseForAuthor(Guid authorId, Guid courseId)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
                return NotFound();

            var course = _courseLibraryRepository.GetCourse(authorId, courseId);

            if (course == null)
                return NotFound();

            return Ok(_mapper.Map<CourseDto>(course));
        }

        [HttpPost]
        public IActionResult CreateCourseForAuthor([FromRoute]Guid authorId,
                                                   [FromBody] CourseForCreationDto course)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
                return NotFound();

            var courseEntity = _mapper.Map<Course>(course);
            _courseLibraryRepository.AddCourse(authorId, courseEntity);
            _courseLibraryRepository.Save();

            var courseToReturn = _mapper.Map<CourseDto>(courseEntity);

            return CreatedAtRoute(nameof(GetCourseForAuthor),
                         new { authorId = authorId, courseId = courseToReturn.Id },
                         courseToReturn);
        }

        [HttpPut("{courseId}")]
        public IActionResult UpdateCourseForAuthor(Guid authorId,
                                                  Guid courseId,
                                                  CourseForUpdateDto course)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
                return NotFound();

            var courseFromRepo = _courseLibraryRepository.GetCourse(authorId, courseId);

            if (courseFromRepo == null)
            {
                var courseToAdd = _mapper.Map<Course>(course);
                courseToAdd.Id = courseId;

                _courseLibraryRepository.AddCourse(authorId, courseToAdd);
                _courseLibraryRepository.Save();

                var courseToReturn = _mapper.Map<CourseDto>(courseToAdd);

                return CreatedAtRoute(nameof(GetCourseForAuthor),
                        new { authorId, courseId = courseToReturn.Id },
                        courseToReturn);
            }

            _mapper.Map(course, courseFromRepo);

            _courseLibraryRepository.UpdateCourse(courseFromRepo);
            _courseLibraryRepository.Save();

            return NoContent();
        }

        [HttpPatch("{courseId}")]
        public IActionResult PartiallyUpdateCourseForAuthor(Guid authorId,
                                                            Guid courseId,
                                                            JsonPatchDocument<CourseForUpdateDto> patchDocument)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
                return NotFound();

            var courseFromRepo = _courseLibraryRepository.GetCourse(authorId, courseId);

            if (courseFromRepo == null)
                return NotFound();

            var courseToPatch = _mapper.Map<CourseForUpdateDto>(courseFromRepo);
            patchDocument.ApplyTo(courseToPatch); //TODO: addvalidation
            _mapper.Map(courseToPatch, courseFromRepo);

            _courseLibraryRepository.UpdateCourse(courseFromRepo);
            _courseLibraryRepository.Save();

            return NoContent();
        }
    }
}