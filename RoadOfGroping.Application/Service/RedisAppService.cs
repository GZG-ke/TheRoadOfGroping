﻿using RoadOfGroping.Common.Attributes;
using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Application.Service
{
    /// <summary>
    /// redis测试服务
    /// </summary>
    [DisabledUnitOfWork(true)]
    public class RedisAppService : ApplicationService
    {
        private readonly ICacheTool cacheTool;

        public RedisAppService(ICacheTool cacheTool)
        {
            this.cacheTool = cacheTool;
        }

        public async Task SetRedis(string key, string val)
        {
            await cacheTool.SetAsync(key, val);
        }

        public async Task<string> GetRedis(string key)
        {
            return await cacheTool.GetAsync(key);
        }
    }
}