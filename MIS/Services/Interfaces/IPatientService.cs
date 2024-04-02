using Microsoft.AspNetCore.Mvc;
using MIS.Data.DTO;

namespace MIS.Services.Interfaces
{
    public interface IPatientService
    {
        Task<Guid> Register(PatientDTO userRegisterDTO);
        Task<Guid> CreateInspection(CreateInspectionDTO createInspectionDTO, Guid id, Guid IdDoctor);
        Task<ReturnDTOGetPatient> GetPatient(GetPatientDTO userRegisterDTO, Guid id);
        Task<GetCardPatientDTO> GetPatientCard(Guid idPatient);
        Task<List<GetInspectionCardDTO>> GetPatientCardSearch(Guid idPatient, string? code);
        Task<InspectionsResponseDTO> GetInspectionPatient(Guid id, GetPatientInspection getPatientInspection);
    }
}
