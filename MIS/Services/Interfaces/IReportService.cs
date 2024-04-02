using Microsoft.AspNetCore.Mvc;
using MIS.Data.DTO;

namespace MIS.Services.Interfaces
{
    public interface IReportService
    {
        Task<ResponseDTO> GetReport(DTOGetReport dTOGetReport);
    }
}
