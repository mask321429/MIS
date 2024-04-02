using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIS.Data.DTO;
using MIS.Services.Interfaces;
using System.Diagnostics;

namespace MIS.Controllers
{
    [Controller]
    [Route("/api/")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _Report;
        public ReportController(IReportService Report)
        {
            _Report = Report;
        }

   
        [HttpGet]
        [Route("report/isdrootsreport")]
        public async Task<IActionResult> GetConsultation([FromQuery]DTOGetReport dTOGetReport)
        {
            try
            {
                var result = await _Report.GetReport(dTOGetReport);
                return Ok(result);
            }
            catch(Exception ex)
            {
                if (ex.Data.Contains(StatusCodes.Status400BadRequest.ToString()))
                {
                    return BadRequest(ex.Data[StatusCodes.Status400BadRequest.ToString()]);
                }
                if (ex.Data.Contains(StatusCodes.Status404NotFound.ToString()))
                {
                    return BadRequest(ex.Data[StatusCodes.Status404NotFound.ToString()]);
                }
                else
                {
                    Debug.WriteLine("Произошла ошибка: " + ex.Message);
                    return StatusCode(500, "Внутренняя ошибка сервера.");
                }
            }
        }
      
    }
}
