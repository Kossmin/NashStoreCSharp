using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NashPhaseOne.API.Statics;
using NashPhaseOne.BusinessObjects.Models;
using System.Security.Claims;

namespace NashPhaseOne.API.Filters
{
    public class CustomAuthorizeFilter : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        private readonly UserManager<User> _userManager;
        public CustomAuthorizeFilter(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var userClaims = context.HttpContext.User;
            var token = context.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
            if(token != null)
            {
                var id = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var user = await _userManager.FindByIdAsync(id);
                if (user.IsBanned)
                {
                    context.Result = new UnauthorizedObjectResult(new {message = "Your account has been banned"});
                }
            }
            if (!ListOfActiveTokens.ActiveTokens.Contains(token))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
