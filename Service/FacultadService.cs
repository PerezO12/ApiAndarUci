using ApiUci.Dtos;
using ApiUci.Helpers.Querys;
using ApiUci.Interfaces;
using ApiUci.Dtos.Facultad;
using ApiUci.Mappers;

namespace ApiUci.Service
{
    public class FacultadService : IFacultadService
    {
        private readonly IFacultadRepository _facutadRepository;
        private readonly IDepartamentoService _departamentoService;
        private readonly ILogger<FacultadService> _logger;
        public FacultadService(
            IFacultadRepository facultadRepository, 
            IDepartamentoService departamentoService,
            ILogger<FacultadService> logger
            )
        {
            _facutadRepository = facultadRepository;
            _departamentoService = departamentoService;
            _logger = logger;
        }

        public async Task<RespuestasGenerales<FacultadDto>> CreateAsync(FacultadCreateDto facultadDto)
        {
            try
            {
                var facultad = await _facutadRepository.CreateAsync(facultadDto.toFacultadFromCreate());
                return RespuestasGenerales<FacultadDto>.SuccessResponse(facultad.toFacultadDto(), "Facultad creada exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear la facultad. Exception: {ex.Message}");
                throw;
            }
        }

        public async Task<RespuestasGenerales<FacultadDto?>> DeleteAsync(int facultadId)
        {
            try
            {
                var facultad = await _facutadRepository.DeleteAsync(facultadId);
                if(facultad == null)
                    return RespuestasGenerales<FacultadDto?>.ErrorResponseService("Facultad", "La facultad no existe.");
                //borrar los departamentos de la facultad
                await _departamentoService.DeleteAllDepartamentosByFacultad(facultad.Id);
                return RespuestasGenerales<FacultadDto?>.SuccessResponse(facultad.toFacultadDto(), "Operación realizada exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al borrar la facultad con id: {facultadId}. Exception: {ex.Message}");
                throw;
            }
        }

        public async Task<RespuestasGenerales<IEnumerable<FacultadDto>>> GetAllAsync(QueryObjectFacultad query)
        {
            try
            {
                var facultades = await _facutadRepository.GetAllAsync(query);
                var facultadesDto = facultades.Select(f => f.toFacultadDto());
                return RespuestasGenerales<IEnumerable<FacultadDto>>.SuccessResponse(facultadesDto, "Operación realizada exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener las facultades. Exception: {ex.Message}");
                throw;
            }
        }

        public async Task<RespuestasGenerales<FacultadDto?>> GetByIdAsync(int facultadId)
        {
            try
            {
                var facultad = await _facutadRepository.GetByIdAsync(facultadId);
                if(facultad == null)
                    return RespuestasGenerales<FacultadDto?>.ErrorResponseService("Facultad", "La facultad no existe.");
                return RespuestasGenerales<FacultadDto?>.SuccessResponse(facultad.toFacultadDto(), "Operación realizada exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener la facultad con id: {facultadId}. Exception: {ex.Message}");
                throw;
            }
        }

        public async Task<RespuestasGenerales<FacultadDto?>> UpdateAsync(int facultadId, FacultadUpdateDto facultadDto)
        {
            try
            {
                var facultad = await _facutadRepository.UpdateAsync(facultadId, facultadDto.toFacultadFromUpdate());
                if(facultad == null)
                    return RespuestasGenerales<FacultadDto?>.ErrorResponseService("Facultad", "La facultad no existe.");
                return RespuestasGenerales<FacultadDto?>.SuccessResponse(facultad.toFacultadDto(), "Operación realizada exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la facultad con id: {facultadId}. Exception: {ex.Message}");
                throw;
            }
        }
    }
}