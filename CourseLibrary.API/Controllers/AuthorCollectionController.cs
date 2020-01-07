using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("api/authorCollection")]
    public class AuthorCollectionController : ControllerBase
    {

        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;

        public AuthorCollectionController(ICourseLibraryRepository courseLibraryRepository,
                                IMapper mapper)
        {
            _courseLibraryRepository = courseLibraryRepository;
            _mapper = mapper;
        }

        [HttpGet("({ids})", Name = nameof(GetAuthorCollection))]
        public IActionResult GetAuthorCollection(
            [FromRoute]
            [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids
        )
        {
            if (ids == null)
                return BadRequest();

            var authors = _courseLibraryRepository.GetAuthors(ids);

            if (authors.Count() != ids.Count())
                return NotFound();

            var authorsToReturn = _mapper.Map<IEnumerable<AuthorDto>>(authors);

            return Ok(authorsToReturn);
        }

        [HttpPost]
        public IActionResult CreateAuthorCollection([FromBody] IEnumerable<AuthorForCreationDto> authors)
        {
            var authorEntities = _mapper.Map<IEnumerable<Author>>(authors);
            foreach (var author in authorEntities)
            {
                _courseLibraryRepository.AddAuthor(author);
            }

            _courseLibraryRepository.Save();

            var authorsToReturn = _mapper.Map<IEnumerable<AuthorDto>>(authorEntities);
            var authorsIdsAsString = string.Join(",", authorsToReturn.Select(a => a.Id));

            return CreatedAtRoute(nameof(GetAuthorCollection),
                                  new { ids = authorsIdsAsString },
                                  authorsToReturn);
        }
    }
}