using FormBuilder.API.Models;
using FormBuilder.Application.DTOs.ApprovalWorkflow;
using FormBuilder.Domain.Interfaces.Services;
using FormBuilder.Domian.Entitys.FormBuilder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FormBuilder.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administration")]

    public class ApprovalWorkflowController : ControllerBase
    {
        private readonly IApprovalWorkflowService _workflowService;

        public ApprovalWorkflowController(IApprovalWorkflowService workflowService)
        {
            _workflowService = workflowService;
        }

        // GET: api/ApprovalWorkflow
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _workflowService.GetAllAsync();
            return StatusCode(result.StatusCode, result);
        }

        // GET: api/ApprovalWorkflow/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _workflowService.GetByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        // POST: api/ApprovalWorkflow
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ApprovalWorkflowCreateDto dto)
        {
            if (dto == null)
                return BadRequest(new ApiResponse(400, "Invalid request"));

            var result = await _workflowService.CreateAsync(dto);
            return StatusCode(result.StatusCode, result);
        }

        // PUT: api/ApprovalWorkflow/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ApprovalWorkflowUpdateDto dto)
        {
            if (dto == null)
                return BadRequest(new ApiResponse(400, "Invalid request"));

            var result = await _workflowService.UpdateAsync(id, dto);
            return StatusCode(result.StatusCode, result);
        }

        // PATCH: api/ApprovalWorkflow/5/toggle
        [HttpPatch("{id:int}/toggle")]
        public async Task<IActionResult> ToggleActive(int id, [FromQuery] bool isActive)
        {
            var result = await _workflowService.ToggleActiveAsync(id, isActive);
            return StatusCode(result.StatusCode, result);
        }

        // GET: api/ApprovalWorkflow/name/{name}
        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var result = await _workflowService.GetByNameAsync(name);
            return StatusCode(result.StatusCode, result);
        }

        // GET: api/ApprovalWorkflow/active
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var result = await _workflowService.GetActiveAsync();
            return StatusCode(result.StatusCode, result);
        }
        // DELETE: api/ApprovalWorkflow/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _workflowService.DeleteAsync(id);
            return StatusCode(result.StatusCode, result);

        }
    }
}
