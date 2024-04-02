using MIS.Data.DTO;

namespace MIS.Services.Interfaces
{
    public interface IInspectionServise
    {
        Task<FullInfoAboutInspectionDTO> GetFullInspectionById(Guid Id);
        Task UpdateInspection(Guid id, UpdateInspectionDTO updateInspectionDTO, Guid UserId);
        Task<List<ChainDTO>> GetInspectionChain(Guid inspectionId);
    }
}
