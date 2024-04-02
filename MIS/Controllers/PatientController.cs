using MIS.Data.Models;
using MIS.Services.Interfaces;
using MIS.Data.DTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MIS.Services;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace MSI.Controllers
{
    [ApiController]
    [Route("api/patient")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _PatientService;

        public PatientController(IPatientService usersService)
        {
            _PatientService = usersService;
        }


        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] PatientDTO userRegister)
        {
            try
            {

                var guid = await _PatientService.Register(userRegister);
                return Ok(guid);
            }
            catch (ArgumentException ex)
            {

                return BadRequest(ex.Message);
            }
            catch (DbUpdateException ex)
            {
                Debug.WriteLine("Ошибка обновления базы данных: " + ex.Message);
                return StatusCode(400, "Пользователь с такими данными уже существует.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Произошла ошибка: " + ex.Message);
                return StatusCode(500, "Внутренняя ошибка сервера.");
            }
        }
        [Authorize]
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(CreateInspectionDTO userRegister, Guid id)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (userRegister == null ||
                userRegister.CreateInspectionInfo == null ||
                userRegister.CreateInspectionInfo.Date == default ||
                string.IsNullOrWhiteSpace(userRegister.CreateInspectionInfo.Anamnesis) ||
                string.IsNullOrWhiteSpace(userRegister.CreateInspectionInfo.Complaints) ||
                string.IsNullOrWhiteSpace(userRegister.CreateInspectionInfo.Treatment) ||
                !Enum.IsDefined(typeof(Conclusion), userRegister.CreateInspectionInfo.Conclusion) ||
                userRegister.CreateInspectionInfo.Diagnoses == null ||
                userRegister.CreateInspectionInfo.Diagnoses.Count == 0 ||
                userRegister.CreateInspectionInfo.Diagnoses.Any(diagnosis => !Enum.IsDefined(typeof(DiagnosisType), diagnosis.Type)))
            {
                return BadRequest("One or more required fields are empty or contain invalid data.");
            }

            try
            {
                var guid = await _PatientService.CreateInspection(userRegister, id, Guid.Parse(User.Identity.Name));
                return Ok(guid);
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
                    return StatusCode(400, "Пользователь с такими данными уже существует.");
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
        [HttpGet]
        [Route("GetPatient")]
        public async Task<IActionResult> GetPatient([FromQuery] GetPatientDTO getPatientDTO)
        {
         

            try
            {
                var guid = await _PatientService.GetPatient(getPatientDTO, Guid.Parse(User.Identity.Name));
                return Ok(guid);
            }
            catch(Exception ex)
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
                    return StatusCode(400, "Пользователь с такими данными уже существует.");
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
        [HttpGet]
        [Route("GetPatientCard")]
        public async Task<IActionResult> GetPatientCard(Guid Id)
        {


            try
            {
                var guid = await _PatientService.GetPatientCard(Id);
                return Ok(guid);
            }
            catch (Exception ex)
            {
                if (ex.Data.Contains(StatusCodes.Status400BadRequest.ToString()))
                {
                    return BadRequest(ex.Data[StatusCodes.Status400BadRequest.ToString()]);
                }
                else if (ex is DbUpdateException)
                {
                    return StatusCode(400, "Пользователь с такими данными уже существует.");
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
        [HttpGet]
        [Route("GetPatientCardSerch")]
        public async Task<IActionResult> GetPatientCardSearch(Guid Id, string? code)
        {


            try
            {
                var guid = await _PatientService.GetPatientCardSearch(Id, code);
                return Ok(guid);
            }
            catch (Exception ex)
            {
                if (ex.Data.Contains(StatusCodes.Status400BadRequest.ToString()))
                {
                    return BadRequest(ex.Data[StatusCodes.Status400BadRequest.ToString()]);
                }
                else if (ex is DbUpdateException)
                {
                    return StatusCode(400, "Пользователь с такими данными уже существует.");
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
        [HttpGet]
        [Route("patient/{id}/inspection")]
        public async Task<IActionResult> GetInspectionPatient(Guid id, [FromQuery] GetPatientInspection getPatientInspection)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var guid = await _PatientService.GetInspectionPatient(id,getPatientInspection);
                return Ok(guid);
            }
            catch (Exception ex)
            {
                if (ex.Data.Contains(StatusCodes.Status400BadRequest.ToString()))
                {
                    return BadRequest(ex.Data[StatusCodes.Status400BadRequest.ToString()]);
                }
                else if (ex is DbUpdateException)
                {
                    return StatusCode(400, "Пользователь с такими данными уже существует.");
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
    }
}



