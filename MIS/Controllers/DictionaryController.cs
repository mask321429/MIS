using MIS.Data.Models;
using MIS.Services.Interfaces;
using MIS.Data.DTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MSI.Controllers
{
    [ApiController]
    [Route("api/Dictionary")]
    public class DictionaryController : ControllerBase
    {
        private readonly ISpeciality _speciality;

        public DictionaryController(ISpeciality speciality)
        {
            _speciality = speciality;
        }

        [HttpGet]
        [Route("specialities")]
        public async Task<IActionResult> specialityGet([FromQuery] SpecialityGetDTO specialityGetDTO)
        {
            try
            {
                var result = await _speciality.SpecialityGet(specialityGetDTO);
                return Ok(result);
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return BadRequest("An error occurred while processing your request.");
            }
        }
        [HttpGet]
        [Route("icd")]
        public async Task<IActionResult> SpecialityGetNameAndId([FromQuery] SpecialityGetDTO specialityGetDTO)
        {
            try
            {
                if (specialityGetDTO == null)
                {
                    return BadRequest("SpecialityGetDTO is null.");
                }

                var result = await _speciality.SpecialityGetNameAndId(specialityGetDTO);
                return Ok(result);
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return BadRequest("An error occurred while processing your request.");
            }
        }

        [HttpGet]
        [Route("icd/root")]
        public async Task<List<SpecialityWithCodeDTO>> GetRoot()
        {
            var result = await _speciality.GetRoots();
            return result;
        }
    }
}

