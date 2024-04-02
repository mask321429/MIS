using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIS.Data.DTO;
using MIS.Services.Interfaces;
using System.Diagnostics;

namespace MIS.Controllers
{
    [Controller]
    [Route("/api/")]
    public class ConsultationController : ControllerBase
    {
        private readonly IConsultationServise _Consultation;
        public ConsultationController (IConsultationServise consultation)
        {
            _Consultation = consultation;
        }

        [Authorize]
        [HttpGet]
        [Route("consultation")]
        public async Task<IActionResult> GetConsultation([FromQuery]GetPatientInspection getPatientInspection)
        {
            try
            {
                var result = await _Consultation.GetConsultaltatio(Guid.Parse(User.Identity.Name), getPatientInspection);
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
        [Authorize]
        [HttpGet]
        [Route("consultation/{id}")]
        public async Task<IActionResult> GetConsultation([FromRoute]Guid id)
        {
            try
            {
                var consultationDto = await _Consultation.GetConsultation(id);
                return Ok(consultationDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Произошла ошибка на сервере.");
            }
        }


        [Authorize]
        [HttpPost]
        [Route("consultation/{id}/comment")]
        public async Task<IActionResult> AddComment([FromRoute] Guid id, [FromBody] NewCommentDTO newComment)
        {
            try
            {
                var result = await _Consultation.AddComment(id, newComment, Guid.Parse(User.Identity.Name));
                return Ok(result);

            }
            catch(Exception ex)
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
        [HttpPut]
        [Route("consultation/comment/{id}")]
        public async Task<IActionResult> UpdateComment([FromRoute] Guid id, [FromBody] CommentDTO newComment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _Consultation.UpdateComment(id, newComment, Guid.Parse(User.Identity.Name));
                return Ok();

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
