﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Services;
using FreeCourse.Shared.ControllerBases;

namespace FreeCourse.Services.Catalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    internal class CoursesController : CustomBaseController
    {// CustomBaseController den miras alıyorum çünkü hata mesajlarımı oradan veriyorum zaten oda ControllerBase'den miras alıyor
        private readonly ICourseService _courseService;

        internal CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }
        public async Task<IActionResult> GetAll()
        {
            var response = await _courseService.GetAllAsync();
            return CreateActionResultInstance(response);
        }

        
        [HttpGet("{id}")]//eğer bunu belirtmessem bu metoda istek yaptığımda şu şekilde olacaktı "courses?id" onun yerine "courses/id" kullanıyorum
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _courseService.GetByIdAsync(id);

            // Her method da her controllerda bunu böyle kullamak yerine ana bir class libraryde kullandım sonra oradan kalıtım aldım.
            //if (response.StatusCode == 404)
            //{
            //    return NotFound(response.Errors);
            //}

            return CreateActionResultInstance(response);// Hata mesajlarını almak için oluşturduğum custombasecontroller daki method
        }

        
        //[HttpGet("{userId}")] Yukardaki metodla karışmaması için
        //api/courses/getallbyuserid/4 dediğimde bu method çalışacak
        [Route("/api/[controller]/GetAllByUserId/{userId}")]
        public async Task<IActionResult> GetAllByUserId(string userId)
        {
            var response = await _courseService.GetAllByUserIdAsync(userId);
            return CreateActionResultInstance(response);
        }


        [HttpPost]
        public async Task<IActionResult> Create(CourseCreateDto courseCreateDto)
        {
            var response = await _courseService.CreateAsync(courseCreateDto);
            return CreateActionResultInstance(response);
        }


        [HttpPut]
        public async Task<IActionResult> Update(CourseUpdateDto courseUpdateDto)
        {
            var response = await _courseService.UpdateAsync(courseUpdateDto);
            return CreateActionResultInstance(response);
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _courseService.DeleteAsync(id);
            return CreateActionResultInstance(response);
        }
    }
}
