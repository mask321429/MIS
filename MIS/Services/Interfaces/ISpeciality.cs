using MIS.Data.Models;
using MIS.Data.DTO;

namespace MIS.Services.Interfaces
{
    public interface ISpeciality
    {
        Task<SpecialitiesResponseDTO> SpecialityGet(SpecialityGetDTO specialityGetDTO);

        Task<SpecialitiesNameAndIdDTO> SpecialityGetNameAndId(SpecialityGetDTO specialityGetDTO);

        Task<List<SpecialityWithCodeDTO>> GetRoots();
    }
}
