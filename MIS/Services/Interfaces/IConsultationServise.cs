using MIS.Data.DTO;
using MIS.Data.Models;

namespace MIS.Services.Interfaces
{
    public interface IConsultationServise
    {
        Task<InspectionsResponseDTO> GetConsultaltatio(Guid id, GetPatientInspection getPatientInspection);
        Task<ConsultationForGetIdDTO> GetConsultation(Guid consultationId);

        Task<Guid> AddComment(Guid id, NewCommentDTO newComment, Guid IdDoctor);
        Task  UpdateComment(Guid id, CommentDTO newComment, Guid idDoctor);
    }
}
