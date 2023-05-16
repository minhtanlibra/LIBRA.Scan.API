using LIBRA.Scan.Entities.Dtos;
using LIBRA.Scan.Entities.Entities;
using LIBRA.Scan.Service.Constracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json.Serialization;

namespace LIBRA.Scan.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BatchManagerController : ControllerBase
    {
        private readonly ILogger<BatchManagerController> _logger;
        private readonly IBatchService _batchService;
        public BatchManagerController(ILogger<BatchManagerController> logger, IBatchService batchService)
        {
            _logger = logger;
            _batchService = batchService;
        }

        [AllowAnonymous]
        [HttpPost("GetById")]
        public async Task<RequestRespone> Get(long Id)
        {
            try
            {
                BatchDto batch = await _batchService.GetSingle(x => x.Id == Id);

                RequestRespone result = new RequestRespone()
                {
                    CodeError = 0,
                    Content = JsonConvert.SerializeObject(batch, new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    }),
                };
                return result;
            }
            catch (Exception ex)
            {

                return new RequestRespone()
                {
                    CodeError = 2,
                    Message = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message.ToString()
                };
            } 
        }

        [AllowAnonymous]
        [HttpPost("GetByStatus")]
        public async Task<RequestRespone> GetByStatus(long Id)
        {
            try
            {
                IEnumerable<BatchDto> batch = await _batchService.Get(x => x.StatusId == Id);
                RequestRespone result = new RequestRespone()
                {
                    CodeError = 0,
                    Content = JsonConvert.SerializeObject(batch),
                };
                return result;
            }
            catch (Exception ex)
            {

                return new RequestRespone()
                {
                    CodeError = 2,
                    Message = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message.ToString()
                };
            }
        }
        [AllowAnonymous]
        [HttpPost("Add")]
        public async Task<RequestRespone> Add(BatchDto batch)
        {
            if (await _batchService.Add(batch))
            {
                RequestRespone result = new RequestRespone()
                {
                    CodeError = 0,
                    Content = JsonConvert.SerializeObject(true),
                };
                return result;
            }
            return new RequestRespone()
            {
                CodeError = 2,
                Content = JsonConvert.SerializeObject(false),
            };
        }
    }
}