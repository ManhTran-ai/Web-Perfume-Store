using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GuhaStore.Web.Filters;

public class SessionAuthorizationAttribute : Attribute, IAuthorizationFilter
{
    private readonly int[] _allowedAccountTypes;

    public SessionAuthorizationAttribute(params int[] allowedAccountTypes)
    {
        _allowedAccountTypes = allowedAccountTypes;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var accountType = context.HttpContext.Session.GetInt32("AccountType");
        
        if (accountType == null || !_allowedAccountTypes.Contains(accountType.Value))
        {
            context.Result = new RedirectToActionResult("Login", "Account", new { returnUrl = context.HttpContext.Request.Path });
        }
    }
}

