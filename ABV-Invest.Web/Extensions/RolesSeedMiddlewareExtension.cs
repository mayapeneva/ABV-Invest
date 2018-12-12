﻿namespace ABV_Invest.Web.Extensions
{
    using Microsoft.AspNetCore.Builder;

    public static class RolesSeedMiddlewareExtension
    {
        public static IApplicationBuilder UseSeedRolesMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RolesSeedMiddleware>();
        }
    }
}