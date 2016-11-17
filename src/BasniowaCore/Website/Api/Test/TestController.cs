using Microsoft.AspNetCore.Mvc;
using Website.Infrastructure;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Website.Api.Test
{
    /// <summary>
    /// Playground
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        private static AsyncLocal<string> _local = new AsyncLocal<string>();

        [HttpGet]
        public async Task<string> Get()
        {
            
            if (_local.Value == null)
            {
                _local.Value = DateTime.Now.ToString();
            }

            await Task.Yield();

            return _local.Value;
        }
    }
}
