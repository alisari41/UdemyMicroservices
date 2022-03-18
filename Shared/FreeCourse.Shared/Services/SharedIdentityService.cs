using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace FreeCourse.Shared.Services
{
    public class SharedIdentityService : ISharedIdentityService
    { // Kullanıcının Id sini almak için herseferınde GetUserId yazmak yerine bir kere sharedlibraryde yazsak yeter

        private IHttpContextAccessor _httpContextAccessor;

        public SharedIdentityService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Claim kullanıcı hakkında tutulan veriler
        //public string GetUserId => _httpContextAccessor.HttpContext.User.Claims.Where(x => x.Type == "sub").FirstOrDefault().Value;
        public string GetUserId => _httpContextAccessor.HttpContext.User.FindFirst("sub").Value;
    }
}
