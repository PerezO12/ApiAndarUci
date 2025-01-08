using ApiUci.Dtos;
using ApiUci.Interfaces;
using ApiUci.Dtos.Carrera;
using ApiUci.Helpers;
using ApiUci.Mappers;

namespace ApiUci.Service
{
    public class CarreraService : ICarreraService
    {
        private readonly IFacultadRepository _facultadRepository;
        private readonly ICarreraRepository _carreraRepository;
        private readonly ILogger<CarreraService> _logger;

        public CarreraService
        (
            IFacultadRepository facultadRepository,
            ICarreraRepository carreraRepository,
            ILogger<CarreraService> logger
        )
        {
            _facultadRepository = facultadRepository;
            _carreraRepository = carreraRepository;
            _logger = logger;
        }
        public async Task<RespuestasGenerales<CarreraDto>> CreateAsync(CreateCarreraDto carreraDto)
        {
            try
            {
            //veridicar si la facutlade xiste
                if(!await _facultadRepository.FacultyExists(carreraDto.FacultadId))
                    return RespuestasGenerales<CarreraDto>.ErrorResponseService("Facultad", "La facultad no existe.");

                var carreraModel = await _carreraRepository.CreateAsync(carreraDto.toCarreraFromCreate());
                return RespuestasGenerales<CarreraDto>.SuccessResponse(carreraModel.toCarreraDto(), "La carrera fue creada exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error al crear la carrera.");
                throw;
            }
        }

        public async Task<RespuestasGenerales<CarreraDto>> DeleteAsync(int carreraId)
        {
            try
            {
                var carrera = await _carreraRepository.DeleteAsync(carreraId);
                if(carrera == null)
                    return RespuestasGenerales<CarreraDto>.ErrorResponseService("Carrera", "La carrera no existe.");

                return RespuestasGenerales<CarreraDto>.SuccessResponse(carrera.toCarreraDto(), "La carrera fue creada exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al borrar la carrera {carreraId}.");
                throw;
            }
        }

        public async Task<RespuestasGenerales<IEnumerable<CarreraDto>>> GetAllAsync(QueryObjectCarrera query)
        {
            try
            {
                var carreras =  await _carreraRepository.GetAllAsync(query);
                var carrerasDto = carreras.Select(c => c.toCarreraDto());
                return RespuestasGenerales<IEnumerable<CarreraDto>>.SuccessResponse(carrerasDto, "Operación realizada exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error al obtener las carreras.");
                throw;
            }
        }

        public async Task<RespuestasGenerales<CarreraDto>> GetByIdAsync(int carreraId)
        {
            try
            {
                var carrera = await _carreraRepository.GetByIdAsync(carreraId);
                if(carrera == null)
                    return RespuestasGenerales<CarreraDto>.ErrorResponseService("Carrera", "La carrera no existe."); 

                return RespuestasGenerales<CarreraDto>.SuccessResponse(carrera.toCarreraDto(), "La Operación fue realizada exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener la carrera {carreraId}.");
                throw;
            }
        }

        public async Task<RespuestasGenerales<CarreraDto>> PatchAsync(int carreraId, PatchCarreraDto carreraDto)
        {
            try
            {
                var carrera = await _carreraRepository.PatchAsync(carreraId, carreraDto);
                if(carrera == null)
                    return RespuestasGenerales<CarreraDto>.ErrorResponseService("Carrera", "La carrera no existe."); 

                return RespuestasGenerales<CarreraDto>.SuccessResponse(carrera.toCarreraDto(), "La Operación fue realizada exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar la carrera {carreraId}.");
                throw;
            }
        }

        public async Task<RespuestasGenerales<CarreraDto>> UpdateAsync(int carreraId, UpdateCarreraDto updateCarreraDto)
        {
            try
            {
                var carrera = await _carreraRepository.UpdateAsync(carreraId, updateCarreraDto.toCarreraFromUpdate());
                if(carrera == null)
                    return RespuestasGenerales<CarreraDto>.ErrorResponseService("Carrera", "La carrera no existe."); 

                return RespuestasGenerales<CarreraDto>.SuccessResponse(carrera.toCarreraDto(), "La Operación fue realizada exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar la carrera {carreraId}.");
                throw;
            }
        }
    }
}