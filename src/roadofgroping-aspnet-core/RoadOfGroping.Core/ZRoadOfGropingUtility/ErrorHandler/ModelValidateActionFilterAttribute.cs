﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.ErrorHandler
{
    /// <summary>
    /// 模型验证
    /// </summary>
    public class ModelValidateActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                //获取具体的错误消息
                // 获取失败的验证信息列表
                var errors = context.ModelState
                    .Where(s => s.Value != null && s.Value.ValidationState == ModelValidationState.Invalid)
                    .Select(e => new
                    {
                        Key = e.Key,
                        Message = e.Value.Errors.Select(p => p.ErrorMessage)
                    }).ToDictionary(p => p.Key, c => c.Message);

                context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

                context.Result = new BadRequestObjectResult(errors);
            }
        }
    }
}