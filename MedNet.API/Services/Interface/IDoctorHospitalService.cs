using MedNet.API.Models.DTO;

namespace MedNet.API.Services.Interface
{
    public interface IDoctorHospitalService
    {
        Task BindDoctorToHospitalAsync(Guid doctorId, Guid hospitalId);
        Task UnbindDoctorFromHospitalAsync(Guid doctorId, Guid hospitalId);
        Task<IEnumerable<DoctorHospitalDto>> GetDoctorsByHospitalAsync(Guid hospitalId);
        Task<IEnumerable<DoctorHospitalDto>> GetHospitalsByDoctorAsync(Guid doctorId);
    }

}
