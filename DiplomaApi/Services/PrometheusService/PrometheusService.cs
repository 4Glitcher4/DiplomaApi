using Microsoft.AspNetCore.Authorization;

namespace DiplomaApi.Services
{
    public class PrometheusService : AuthorizationHandler<PrometheusService>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PrometheusService requirement)
        {
            try
            {

                var httpContext = context.Resource as HttpContext;
                var settings = httpContext.RequestServices.GetService<IPrometheusSettings>();
                var value = httpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
                if (value != settings.ApiKey)
                    return Task.Run(() =>
                    {
                        return;
                    });
                context.Succeed(requirement);
                return Task.Run(() =>
                {
                    return;
                });
            }
            catch (Exception)
            {
                return Task.Run(() =>
                {
                    return;
                });
            }
        }
    }
}
