using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoadOfGroping.Core.OrderTest;
using RoadOfGroping.Core.OrderTest.Entity;

namespace RoadOfGroping.Host.Controllers
{
    /// <summary>
    /// 测试控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TestController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<TestController> _logger;

        private readonly IOrderRepository orderRepository;

        /// <summary>
        /// 测试控制器
        /// </summary>
        public TestController(ILogger<TestController> logger, IOrderRepository orderRepository)
        {
            _logger = logger;
            this.orderRepository = orderRepository;
        }

        //[Authorize]
        [HttpGet]
        public async Task<ActionResult<List<Order>>> Get(CancellationToken cancellationToken)
        {
            return await orderRepository.Get();

            //return Ok("Summaries");
            //await orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            //return BadRequest();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetTestResult(string userId)
        {
            Console.WriteLine("测试一下输出日志");
            _logger.LogInformation("日志输出了");
            throw new Exception("Test exception");
            //return Ok(_user);
        }
    }
}