using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeCourse.IdentityServer.Dtos;
using FreeCourse.IdentityServer.Models;
using FreeCourse.Shared.Dtos;
using Microsoft.AspNetCore.Identity;

namespace FreeCourse.IdentityServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpDto signUpDto)
        {

            var user = new ApplicationUser
            {
                UserName = signUpDto.UserName,
                Email = signUpDto.Email,
                City = signUpDto.City
                //Password'ü burada vermiyorum onu aşağıda CreateAsync de  hashing yapıcam
            };

            var result = await _userManager.CreateAsync(user, signUpDto.Password);

            if (!result.Succeeded)
            {//400 kodu client'a meydana gelen hatayı belirtir
                return BadRequest(Response<NoContent>.Fail(result.Errors.Select(x => x.Description).ToList(), 400));
            }

            return NoContent();
        }
    }
}
