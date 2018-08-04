﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Dasdaq.Dev.Agent.Models;
using Dasdaq.Dev.Agent.Services;

namespace Dasdaq.Dev.Agent.Controllers.Api
{
    [Route("api/[controller]")]
    public class InstanceController : BaseController
    {
        [HttpGet]
        public ApiResult<IEnumerable<Instance>> Get([FromServices] AgentContext ef)
        {
            var instances = ef.Instances.ToList();
            return ApiResult<IEnumerable<Instance>>(instances);
        }
        
        [HttpGet("{id}")]
        public object Get(string id, [FromServices] AgentContext ef)
        {
            var instance = ef.Instances.SingleOrDefault(x => x.Name == id);
            if (instance == null)
            {
                return ApiResult(404, "Not Found");
            }

            return ApiResult(instance);
        }

        [HttpPut("{id}")]
        [HttpPost("{id}")]
        [HttpPatch("{id}")]
        public ApiResult Put(string id, [FromBody] PutInstanceRequestBody request,
            [FromServices] AgentContext ef, [FromServices] IServiceProvider services)
        {
            if (_dic.ContainsKey(id))
            {
                return ApiResult(409, "The instance is already existed.");
            }

            ef.Instances.Add(new Instance
            {
                UploadMethod = request.Method,
                Data = request.Data,
                Name = id,
                Status = InstanceStatus.Running
            });
            ef.SaveChanges();

            Task.Run(() => {
                using (var serviceScope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
                using (var _ins = serviceScope.ServiceProvider.GetService<InstanceService>())
                {
                    _ins.DownloadAndStartInstanceAsync(id, request.Method, request.Data);
                }
            });

            return ApiResult(201, "Created");
        }

        [HttpDelete("{id}")]
        public ApiResult Delete(string id, [FromServices] AgentContext ef)
        {
            var instance = ef.Instances.SingleOrDefault(x => x.Name == id);
            if (instance == null)
            {
                return ApiResult(404, "Not Found");
            }

            if (_dic.ContainsKey(id))
            {
                _dic[id].Kill();
                _dic.Remove(id);
            }

            ef.Remove(instance);
            ef.SaveChanges();

            return ApiResult(200, "Succeeded");
        }
    }
}