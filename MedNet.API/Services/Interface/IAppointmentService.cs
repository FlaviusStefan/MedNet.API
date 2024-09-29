using MedNet.API.Models.DTO;

namespace MedNet.API.Services.Interface
{
    public interface IAppointmentService
    {
        Task<AppointmentDto> CreateAppointmentAsync(CreateAppointmentRequestDto request);
        Task<IEnumerable<AppointmentDto>> GetAllAppointmentsAsync();
        Task<AppointmentDto?> GetAppointmentByIdAsync(Guid id);
        Task<AppointmentDto?> UpdateAppointmentAsync(Guid id, UpdateAppointmentRequestDto request);
        Task<AppointmentDto?> DeleteAppointmentAsync(Guid id);
    }
}
