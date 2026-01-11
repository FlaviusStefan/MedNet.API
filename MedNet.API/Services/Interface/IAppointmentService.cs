using MedNet.API.Models.DTO;

namespace MedNet.API.Services.Interface
{
    public interface IAppointmentService
    {
        Task<CreatedAppointmentDto> CreateAppointmentAsync(CreateAppointmentRequestDto request);
        Task<IEnumerable<AppointmentSummaryDto>> GetAllAppointmentsAsync();
        Task<AppointmentDetailDto?> GetAppointmentByIdAsync(Guid id);
        Task<UpdatedAppointmentDto?> UpdateAppointmentAsync(Guid id, UpdateAppointmentRequestDto request);
        Task<string?> DeleteAppointmentAsync(Guid id);
    }
}
