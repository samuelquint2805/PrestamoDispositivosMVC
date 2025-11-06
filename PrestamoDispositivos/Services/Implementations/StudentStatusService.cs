using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PrestamoDispositivos.Core;
using PrestamoDispositivos.DataContext.Sections;
using PrestamoDispositivos.DTO;
using PrestamoDispositivos.Models;
using PrestamoDispositivos.Services.Abstractions;

namespace PrestamoDispositivos.Services.Implementations
{
    public class StudentStatusService : IStudentStatusService
    {
        private readonly DatacontextPres _context;
        private readonly IMapper _mapper;

        public StudentStatusService(DatacontextPres context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Obtener todos los dispositivos
        public async Task<Response<List<studentStatusDTO>>> GetAllStudentStaAsync()
        {
            try
            {
                var StudentStaDV = await _context.EstadoEstudiantes
                    .ToListAsync();

                var StudentDTO = _mapper.Map<List<studentStatusDTO>>(StudentStaDV);

                return new Response<List<studentStatusDTO>>(
                    StudentDTO,
                    "Estado del estudiante obtenidos correctamente"
                );
            }
            catch (Exception ex)
            {
                return new Response<List<studentStatusDTO>>(
                    "Error al obtener lista del estado de los Estudiantes",
                    new List<string> { ex.Message }
                );
            }
        }

        // Obtener Estudiante por ID
        public async Task<Response<studentStatusDTO>> GetStudentStaByIdAsync(Guid id)
        {
            try
            {
                var StudentStaGT = await _context.EstadoEstudiantes
                    .Include(x => x.Prestamos)
                    .FirstOrDefaultAsync(x => x.IdStatus == Guid.Parse(id.ToString()));

                if (StudentStaGT == null)
                    return new Response<studentStatusDTO>("Estado del Estudiante no encontrado");

                var StudentStaDto = _mapper.Map<studentStatusDTO>(StudentStaGT);

                return new Response<studentStatusDTO>(
                    StudentStaDto,
                    "Estado del Estudiante encontrado correctamente"
                );
            }
            catch (Exception ex)
            {
                return new Response<studentStatusDTO>(
                    "Error al obtener el Estado del Estudiante",
                    new List<string> { ex.Message }
                );
            }
        }

        // Crear nuevo Estudiante
        public async Task<Response<studentStatusDTO>> CreateStudentStaAsync(studentStatusDTO StudentStaDto)
        {
            try
            {

                // Verificar si el usuario ya existe
                var existingUser = await _context.EstadoEstudiantes
                    .FirstOrDefaultAsync(x => x.IdStatus == StudentStaDto.IdStatus);

                if (existingUser != null)
                    return new Response<studentStatusDTO>("El estado del estudiante ya existe");

                // Mapear DTO a modelo
                var studentsSta = _mapper.Map<studentStatus>(StudentStaDto);
                StudentStaDto.IdStatus = Guid.NewGuid();

                // Guardar en base de datos
                _context.EstadoEstudiantes.Add(studentsSta);
                await _context.SaveChangesAsync();

                // Mapear resultado
                var resultDto = _mapper.Map<studentStatusDTO>(studentsSta);

                return new Response<studentStatusDTO>(
                    resultDto,
                    "Estado del Estudiante creado correctamente"
                );
            }
            catch (Exception ex)
            {
                return new Response<studentStatusDTO>(
                    "Error al crear el estado del estudiante",
                    new List<string> { ex.Message }
                );
            }
        }

        // Actualizar Estudiante
        public async Task<Response<studentStatusDTO>> UpdateStudentStaAsync(Guid id, studentStatusDTO StudentDto)
        {
            try
            {
                var StudentUP = await _context.EstadoEstudiantes
                    .Include(x => x.Prestamos)
                    .FirstOrDefaultAsync(x => x.IdStatus== Guid.Parse(id.ToString()));

                if (StudentUP == null)
                    return new Response<studentStatusDTO>("estado del estudiante no encontrado");

               

                // Actualizar propiedades

                _mapper.Map(StudentDto, StudentUP);

                _context.EstadoEstudiantes.Update(StudentUP);
                await _context.SaveChangesAsync();

                var resultDto = _mapper.Map<studentStatusDTO>(StudentUP);

                return new Response<studentStatusDTO>(
                    resultDto,
                    "Estado del estudiante actualizado correctamente"
                );
            }
            catch (Exception ex)
            {
                return new Response<studentStatusDTO>(
                    "Error al actualizar el Estudiante",
                    new List<string> { ex.Message }
                );
            }
        }

        // Eliminar Dispositivo
        public async Task<Response<bool>> DeleteStudentStaAsync(Guid id)
        {
            try
            {
                var StudentStaDt = await _context.EstadoEstudiantes
                   .Include(x => x.Prestamos)
                   .FirstOrDefaultAsync(x => x.IdStatus == Guid.Parse(id.ToString()));

                if (StudentStaDt == null)
                    return new Response<bool>("Estado del estudiante no encontrado");

                // Validar si tiene préstamos asociados
                if (StudentStaDt.Prestamos != null && StudentStaDt.Prestamos.Any())
                {
                    return new Response<bool>(
                        "No se puede eliminar el estado del estudiante porque tiene algun estudiante asociado"
                    );
                }

                _context.EstadoEstudiantes.Remove(StudentStaDt);
                await _context.SaveChangesAsync();

                return new Response<bool>(true, "Estado del Estudiante eliminado correctamente");
            }
            catch (Exception ex)
            {
                return new Response<bool>(
                    "Error al eliminar el estado del estudiante",
                    new List<string> { ex.Message }
                );
            }
        }
    }
}
