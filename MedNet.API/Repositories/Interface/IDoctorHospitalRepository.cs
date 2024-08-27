using MedNet.API.Models.Domain;

namespace MedNet.API.Repositories.Interface
{
    public interface IDoctorHospitalRepository
    {
        Task BindAsync(DoctorHospital doctorHospital);
        Task UnbindAsync(Guid doctorId, Guid hospitalId);
        Task<DoctorHospital?> GetBindingAsync(Guid doctorId, Guid hospitalId);
        Task<IEnumerable<DoctorHospital>> GetDoctorsByHospitalAsync(Guid hospitalId);
        Task<IEnumerable<DoctorHospital>> GetHospitalsByDoctorAsync(Guid doctorId);
    }

}
