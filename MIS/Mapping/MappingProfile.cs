using AutoMapper;
using MIS.Data.Models;
using MIS.Data.DTO;

namespace DeliveryBackend.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
 
        CreateMap<UserRegisterDTO, User>();
        CreateMap<User, UserDTO>();
        CreateMap<SpecialityGetModel, SpecialityGetDTO>();
    
    }
}