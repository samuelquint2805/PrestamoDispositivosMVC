using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PrestamoDispositivos.Core;
using PrestamoDispositivos.Data;
using PrestamoDispositivos.DataContext.Sections;
using PrestamoDispositivos.DTO;
using PrestamoDispositivos.Models;
using PrestamoDispositivos.Services.Abstractions;

namespace PrestamoDispositivos.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly DatacontextPres _context;
        private readonly IMapper _mapper;

        public StudentService(DatacontextPres context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        //  Obtener todos los estudiantes
        public async Task<Response<List<StudentDTO>>> GetAllStudentsAsync()
        {
            try
            {
                var students = await _context.Estudiante
                    .Include(x => x.Prestamos)
                    .ToListAsync();

                var studentDTOs = _mapper.Map<List<StudentDTO>>(students);

                return new Response<List<StudentDTO>>(studentDTOs, "Estudiantes obtenidos correctamente");
            }
            catch (Exception ex)
            {
                return new Response<List<StudentDTO>>("Error al obtener lista de estudiantes",
                    new List<string> { ex.Message });
            }
        }

        //  Obtener estudiante por ID
        public async Task<Response<StudentDTO>> GetStudentByIdAsync(Guid id)
        {
            try
            {
                var student = await _context.Estudiante
                    .Include(x => x.Prestamos)
                    .FirstOrDefaultAsync(x => x.IdEst == id);

                if (student == null)
                    return new Response<StudentDTO>("Estudiante no encontrado");

                var studentDto = _mapper.Map<StudentDTO>(student);

                return new Response<StudentDTO>(studentDto, "Estudiante encontrado correctamente");
            }
            catch (Exception ex)
            {
                return new Response<StudentDTO>("Error al obtener el estudiante", new List<string> { ex.Message });
            }
        }

        //  Crear nuevo estudiante
        public async Task<Response<StudentDTO>> CreateStudentAsync(StudentDTO studentDto)
        {
            try
            {
                // Generar nuevo ID antes del mapeo
                studentDto.IdEst = Guid.NewGuid();

                // Mapear DTO a entidad
                var student = _mapper.Map<Student>(studentDto);

                // Guardar en base de datos
                _context.Estudiante.Add(student);
                await _context.SaveChangesAsync();

                var resultDto = _mapper.Map<StudentDTO>(student);

                return new Response<StudentDTO>(resultDto, "Estudiante creado correctamente");
            }
            catch (Exception ex)
            {
                return new Response<StudentDTO>("Error al crear el estudiante", new List<string> { ex.Message });
            }
        }

        //  Actualizar estudiante
        public async Task<Response<StudentDTO>> UpdateStudentAsync(Guid id, StudentDTO studentDto)
        {
            try
            {
                var student = await _context.Estudiante
                    .Include(x => x.Prestamos)
                    .FirstOrDefaultAsync(x => x.IdEst == id);

                if (student == null)
                    return new Response<StudentDTO>("Estudiante no encontrado");

                // Actualizar propiedades
                _mapper.Map(studentDto, student);

                _context.Estudiante.Update(student);
                await _context.SaveChangesAsync();

                var resultDto = _mapper.Map<StudentDTO>(student);

                return new Response<StudentDTO>(resultDto, "Estudiante actualizado correctamente");
            }
            catch (Exception ex)
            {
                return new Response<StudentDTO>("Error al actualizar el estudiante", new List<string> { ex.Message });
            }
        }

        //  Eliminar estudiante
        public async Task<Response<bool>> DeleteStudentAsync(Guid id)
        {
            try
            {
                var student = await _context.Estudiante
                    .Include(x => x.Prestamos)
                    .FirstOrDefaultAsync(x => x.IdEst == id);

                if (student == null)
                    return new Response<bool>("Estudiante no encontrado");

                // Validar si tiene préstamos asociados
                if (student.Prestamos != null && student.Prestamos.Any())
                {
                    return new Response<bool>("No se puede eliminar el estudiante porque tiene préstamos asociados");
                }

                _context.Estudiante.Remove(student);
                await _context.SaveChangesAsync();

                return new Response<bool>(true, "Estudiante eliminado correctamente");
            }
            catch (Exception ex)
            {
                return new Response<bool>("Error al eliminar el estudiante", new List<string> { ex.Message });
            }
        }
    }
}
