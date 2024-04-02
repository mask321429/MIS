using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIS.Data.DTO;
using MIS.Services;
using MIS.Services.Interfaces;
using System.Diagnostics;

namespace MIS.Controllers
{
    [ApiController]
    [Route("api/inspection")]
    public class InspecrionController : ControllerBase
    {
        private readonly IInspectionServise _inspectionServise;
        public InspecrionController(IInspectionServise inspectionServise)
        {
            _inspectionServise = inspectionServise;
        }
        [Authorize]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> FullInspection([FromRoute] Guid id)
        {
            try
            {
                var inspectionDto = await _inspectionServise.GetFullInspectionById(id);
                return Ok(inspectionDto);
            }
            catch (Exception ex)
            {
                if (ex.Data.Contains(StatusCodes.Status400BadRequest.ToString()))
                {
                    return BadRequest(ex.Data[StatusCodes.Status400BadRequest.ToString()]);
                }
                else if (ex is ArgumentException)
                {
                    return BadRequest(ex.Message);
                }
                else if (ex is DbUpdateException)
                {
                    Debug.WriteLine("Ошибка обновления базы данных: " + ex.Message);
                    return StatusCode(400, "Ошибка при обработке данных.");
                }
                else if (ex.Data.Contains(StatusCodes.Status404NotFound.ToString()))
                {
                    return NotFound(ex.Data[StatusCodes.Status404NotFound.ToString()]);
                }
                else
                {
                    Debug.WriteLine("Произошла ошибка: " + ex.Message);
                    return StatusCode(500, "Внутренняя ошибка сервера.");
                }
            }
        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInspection(Guid id, [FromBody] UpdateInspectionDTO updateInspectionDTO)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            if (token == null)
            {
                return BadRequest("Access token not found in the current context.");
            }
            try
            {
                await _inspectionServise.UpdateInspection(id, updateInspectionDTO, Guid.Parse(User.Identity.Name));
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                if (ex.Data.Contains(StatusCodes.Status400BadRequest.ToString()))
                {
                    return BadRequest(ex.Data[StatusCodes.Status400BadRequest.ToString()]);
                }
                else if (ex.Data.Contains(StatusCodes.Status404NotFound.ToString()))
                {
                    return NotFound(ex.Data[StatusCodes.Status404NotFound.ToString()]);
                }
                return StatusCode(500, "Внутренняя ошибка сервера.");
            }
        }

        [Authorize]
        [HttpGet]
        [Route("chain/{id}")]
        public async Task<IActionResult> GetInspectionChain([FromRoute] Guid id)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            if (token == null)
            {
                return BadRequest("Access token not found in the current context.");
            }
            try
            {
               var chain = await _inspectionServise.GetInspectionChain(id);
                return Ok(chain);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                if (ex.Data.Contains(StatusCodes.Status400BadRequest.ToString()))
                {
                    return BadRequest(ex.Data[StatusCodes.Status400BadRequest.ToString()]);
                }
                else if (ex.Data.Contains(StatusCodes.Status404NotFound.ToString()))
                {
                    return NotFound(ex.Data[StatusCodes.Status404NotFound.ToString()]);
                }
                return StatusCode(500, "Внутренняя ошибка сервера.");
            }
        }
    }
}
