﻿using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using RoadOfGroping.Repository.UserSession;

namespace RoadOfGroping.Repository.DomainService
{
    public class ServiceBase
    {
        /// <summary>
        /// 用户信息
        /// </summary>
        public IUserSession UserService { get; set; }

        /// <summary>
        /// 对象映射
        /// </summary>
        public IMapper ObjectMapper { get; set; }

        public ServiceBase(IServiceProvider serviceProvider)
        {
            ObjectMapper = serviceProvider.GetRequiredService<IMapper>();
            UserService = serviceProvider.GetRequiredService<IUserSession>();
        }
    }
}