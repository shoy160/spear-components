﻿using Spear.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Spear.Sharp.Contracts.Enums;
using Spear.Sharp.Domain;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Spear.Sharp.Filters
{
    /// <summary>
    /// 两种认证方式
    /// 1.项目编码 (参数)
    /// 2.Authorization (Header)
    /// </summary>
    public class SpearAuthorizeAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionDescriptor.EndpointMetadata.Any(t => t is AllowAnonymousAttribute))
            {
                await base.OnActionExecutionAsync(context, next);
                return;
            }

            //禁止访问
            void Forbidden()
            {
                context.Result =
                    new JsonResult(DResult.Error("Forbidden", 401))
                    {
                        StatusCode = (int)HttpStatusCode.Unauthorized
                    };
            }

            var httpContext = context.HttpContext;
            //方式一 参数/header project
            var project = httpContext.GetProjectByCode();
            if (project != null)
            {
                if (project.Security == SecurityEnum.None)
                {
                    httpContext.SetProject(project);
                    await base.OnActionExecutionAsync(context, next);
                    return;
                }
                //获取时不需要认证
                if ((project.Security & SecurityEnum.Get) == 0)
                {
                    if (context.ActionDescriptor is ControllerActionDescriptor action &&
                        action.FilterDescriptors.Any(t => t.Filter.GetType() == typeof(AllowGetAttribute)))
                    {
                        httpContext.SetProject(project);
                        await base.OnActionExecutionAsync(context, next);
                        return;
                    }
                }
            }

            //方式二 Header Authorization
            project = httpContext.GetProjectByToken();
            if (project == null)
            {
                Forbidden();
                return;
            }
            httpContext.SetProject(project);
            await base.OnActionExecutionAsync(context, next);
        }
    }
}
