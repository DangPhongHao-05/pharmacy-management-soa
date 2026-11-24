// Controllers/ImportReceiptsController.cs
using Core_Service.DTOs.Imports;
using Core_Service.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Core_Service.Controllers
{
    [ApiController]
    [Route("api/import-receipts")]
    public class ImportReceiptsController : ControllerBase
    {
        private readonly IImportService _importService;

        public ImportReceiptsController(IImportService importService)
        {
            _importService = importService;
        }

        // Helper lấy UserId từ JWT Token (tạm fallback về 1 nếu chưa gắn Auth)
        private long GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return long.TryParse(claim, out var userId) ? userId : 1;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] long? supplierId, [FromQuery] string? status)
        {
            var result = await _importService.GetAllAsync(supplierId, status);
            return Ok(result);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _importService.GetByIdAsync(id);
            return Ok(result);
        }

        //tạo phiếu nhập nháp
        [HttpPost]
        public async Task<IActionResult> CreateDraft([FromBody] CreateImportRequestDto dto)
        {
            var userId = GetCurrentUserId();
            var result = await _importService.CreateDraftAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        //sửa phiếu nhập nháp
        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateDraft(long id, [FromBody] CreateImportRequestDto dto)
        {
            var result = await _importService.UpdateDraftAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteDraft(long id)
        {
            await _importService.DeleteDraftAsync(id);
            return NoContent();
        }

        //duyet phieu nhap kho
        [HttpPost("{id:long}/confirm")]
        public async Task<IActionResult> Confirm(long id)
        {
            var userId = GetCurrentUserId();
            await _importService.ConfirmImportAsync(id, userId);
            return Ok(new { message = "Xác nhận phiếu nhập và cập nhật tồn kho thành công." });
        }
    }
}