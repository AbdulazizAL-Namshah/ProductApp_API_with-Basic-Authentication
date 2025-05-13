using ProductApp_API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ProductApp_API.Authorization;

public class PermissionBasedAuthorizationFilter(ApplicationDbContext dbContext) : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var attribute = (CheckPermissionAttribute)context.ActionDescriptor.EndpointMetadata.FirstOrDefault(x => x is CheckPermissionAttribute);
        if (attribute != null)
        {
            var claimIdentity = context.HttpContext.User.Identity as ClaimsIdentity;
            if (claimIdentity == null || !claimIdentity.IsAuthenticated)
            {
                context.Result = new ForbidResult();
            }
            else
            {
                var userId = int.Parse(claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);
                var hasPermission = dbContext.Set<UserPermission>().Any(x => x.UserId == userId && x.PermissionId == attribute.Permission);
                    if(!hasPermission){
                    context.Result = new ForbidResult();
                }
            }
        }
    }
}

























//public void OnAuthorization(AuthorizationFilterContext context)
//{
//    var attribute = (CheckPermissionAttribute)context.ActionDescriptor.EndpointMetadata.FirstOrDefault(x => x is CheckPermissionAttribute);
//    if (attribute != null)
//    {
//        var claimIdentity = context.HttpContext.User.Identity as ClaimsIdentity;
//        if (claimIdentity == null || !claimIdentity.IsAuthenticated)
//        {
//            context.Result = new ForbidResult();
//        }
//        else
//        {
//            var userId = int.Parse(claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);
//            var haPermission = dbContext.Set<UserPermission>().Any(x => x.UserId == userId && x.PermissionId == attribute.Permission);
//            if (!haPermission)
//            {
//                context.Result = new ForbidResult();
//            }
//        }
//    }
//}