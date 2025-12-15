using FormBuilder.Domian.Entitys.FormBuilder;
using FormBuilder.Application.DTOs.ApprovalWorkflow;
using FormBuilder.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace FormBuilder.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administration")]
    public class ApprovalStageController : ControllerBase
    {
        private readonly IApprovalStageService _service;

        public ApprovalStageController(IApprovalStageService service)
        {
            _service = service;
        }

        // GET: api/ApprovalStage/workflow/1
        [HttpGet("workflow/{workflowId}")]
        public async Task<IActionResult> GetAll(int workflowId)
        {
            var response = await _service.GetAllAsync(workflowId);
            return StatusCode(response.StatusCode, response);
        }

        // GET: api/ApprovalStage/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _service.GetByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        // POST: api/ApprovalStage
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ApprovalStageCreateDto dto)
        {
            var response = await _service.CreateAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        // PUT: api/ApprovalStage/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ApprovalStageUpdateDto dto)
        {
            var response = await _service.UpdateAsync(id, dto);
            return StatusCode(response.StatusCode, response);
        }

        // PATCH: api/ApprovalStage/5/toggle-active
        [HttpPatch("{id}/toggle-active")]
        public async Task<IActionResult> ToggleActive(int id, [FromQuery] bool isActive)
        {
            var response = await _service.ToggleActiveAsync(id, isActive);
            return StatusCode(response.StatusCode, response);
        }

        // DELETE: api/ApprovalStage/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _service.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}
