using All4GYM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace All4GYM.Attributes;

public class MinimumSubscriptionAttribute : Attribute, IAuthorizationFilter
{
    private readonly SubscriptionTier _requiredTier;

    public MinimumSubscriptionAttribute(SubscriptionTier requiredTier)
    {
        _requiredTier = requiredTier;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new RedirectToPageResult("/Login");
            return;
        }

        var tierClaim = user.FindFirst("SubscriptionTier")?.Value;
        if (tierClaim == null || !Enum.TryParse<SubscriptionTier>(tierClaim, true, out var userTier))
        {
            context.Result = new ForbidResult(); // або RedirectToPage("/AccessDenied")
            return;
        }

        if (userTier < _requiredTier)
        {
            context.Result = new RedirectToPageResult("/AccessDenied");
        }
    }
}