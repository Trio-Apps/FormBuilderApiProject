using FormBuilder.Domian.Entitys.froms; // كيان FORM_TABS
using FormBuilder.Core.DTOS.FormTabs; // DTOs لتبادل البيانات
using FormBuilder.Core.IServices.FormBuilder.Services.Services; // واجهة الخدمة IFormTabService
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FormBuilder.API.Controllers
{
    // نقطة الوصول (Route) ووحدة التحكم الأساسية
    [Route("api/FormTabs")]
    [ApiController]
    // تأمين الوحدة: يتطلب دور "Admin"
    [Authorize(Roles = "Admin")]
    public class FormTabsController : ControllerBase
    {
        private readonly IFormTabService _formTabService;

        public FormTabsController(IFormTabService formTabService)
        {
            _formTabService = formTabService;
        }

        // --- Manual Mapping Helper ---
        private FormTabDto MapToDto(FORM_TABS entity)
        {
            if (entity == null) return null;
            return new FormTabDto
            {
                Id = entity.id,
                FormBuilderId = entity.FormBuilderId,
                TabName = entity.TabName,
                TabCode = entity.TabCode,
                TabOrder = entity.TabOrder,
                IsActive = entity.IsActive,
                CreatedByUserId = entity.CreatedByUserId,
                CreatedDate = entity.CreatedDate
            };
        }

        // ----------------------------------------------------------------------
        // --- 1. GET Operations (Read) ---
        // ----------------------------------------------------------------------

        // GET: api/FormTabs/5 (للحصول على تبويب واحد)
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FormTabDto), 200)]
        [ProducesResponseType(404)]
        // 🟢 تم إكمال التنفيذ
        public async Task<IActionResult> GetTabById(int id)
        {
            var tab = await _formTabService.GetTabByIdAsync(id, asNoTracking: true);
            if (tab == null)
            {
                return NotFound();
            }
            return Ok(MapToDto(tab));
        }

        // GET: api/FormTabs/form/1 (للحصول على كل التبويبات لنموذج معين)
        [HttpGet("form/{formBuilderId}")]
        [ProducesResponseType(typeof(IEnumerable<FormTabDto>), 200)]
        public async Task<IActionResult> GetTabsByFormId(int formBuilderId)
        {
            var tabs = await _formTabService.GetTabsByFormIdAsync(formBuilderId);
            var tabsDto = tabs.Select(t => MapToDto(t)).ToList();
            return Ok(tabsDto);
        }

        // ----------------------------------------------------------------------
        // --- 2. POST Operation (Create) ---
        // ----------------------------------------------------------------------
        [HttpPost]
        [ProducesResponseType(typeof(FormTabDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> CreateTab([FromBody] CreateFormTabDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId)) return Unauthorized("User ID not found in claims.");

            try
            {
                var tabEntity = new FORM_TABS
                {
                    FormBuilderId = createDto.FormBuilderId,
                    TabName = createDto.TabName,
                    TabCode = createDto.TabCode,
                    TabOrder = createDto.TabOrder,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    CreatedByUserId = currentUserId
                };

                var createdTab = await _formTabService.CreateTabAsync(tabEntity);
                return CreatedAtAction(nameof(GetTabById), new { id = createdTab.id }, MapToDto(createdTab));
            }
            catch (InvalidOperationException ex)
            {
                // يتم التقاط استثناءات التحقق من المفتاح الخارجي أو تكرار الكود
                if (ex.Message.Contains("does not exist") || ex.Message.Contains("already in use"))
                {
                    return Conflict(ex.Message);
                }
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ----------------------------------------------------------------------
        // --- 3. PUT Operation (Update) ---
        // ----------------------------------------------------------------------
        // PUT: api/FormTabs/5 (تحديث)
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        // 🟢 تم إكمال التنفيذ
        public async Task<IActionResult> UpdateTab(int id, [FromBody] UpdateFormTabDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // جلب الكيان للتأكد من وجوده وتحديثه
            var existingTab = await _formTabService.GetTabByIdAsync(id);
            if (existingTab == null)
            {
                return NotFound();
            }

            try
            {
                // تحديث الخصائص من الـ DTO إلى الكيان الموجود
                existingTab.TabName = updateDto.TabName;
                existingTab.TabCode = updateDto.TabCode;
                existingTab.TabOrder = updateDto.TabOrder;
                existingTab.IsActive = updateDto.IsActive;

                var isUpdated = await _formTabService.UpdateTabAsync(existingTab);

                if (isUpdated)
                {
                    return NoContent(); // HTTP 204
                }
                else
                {
                    return BadRequest("Failed to update the tab due to an unknown service error.");
                }
            }
            catch (InvalidOperationException ex)
            {
                // لمعالجة أخطاء تكرار الكود (Conflict 409)
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ----------------------------------------------------------------------
        // --- 4. DELETE Operation ---
        // ----------------------------------------------------------------------
        // DELETE: api/FormTabs/5 (حذف)
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        // 🟢 تم إكمال التنفيذ
        public async Task<IActionResult> DeleteTab(int id)
        {
            var isDeleted = await _formTabService.DeleteTabAsync(id);

            if (!isDeleted)
            {
                // إذا لم يتم العثور على السجل للحذف
                return NotFound();
            }

            return NoContent(); // HTTP 204
        }
    }
}