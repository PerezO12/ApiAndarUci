using ApiUci.Dtos.Estudiante;
using ApiUci.Helpers;
using ApiUci.Models;

namespace ApiUci.Interfaces
{
    public interface IEstudianteRepository
    {
        Task<List<Estudiante>> GetAllAsync(QueryObjectEstudiante query);
        Task<Estudiante?> GetByIdAsync(int id);
        Task<Estudiante?> GetEstudianteByUserId(string id);
        Task<Estudiante> CreateAsync(Estudiante estudianteModel);
        Task<Estudiante?> UpdateAsync(int id, EstudianteUpdateDto estudianteDto);
        Task<Estudiante?> UpdateEstudianteByUserIdAsync(string id, EstudianteUpdateDto estudianteDto);
        Task<Estudiante?> DeleteAsync(int id);
        Task<Estudiante?> DeleteByUserIdAsync(string userId);
    }
}