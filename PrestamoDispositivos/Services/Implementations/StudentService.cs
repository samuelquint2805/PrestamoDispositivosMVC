using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PrestamoDispositivos.Core;
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

        // Obtener todos los dispositivos
        public async Task<Response<List<StudentDTO>>> GetAllStudentsAsync()
        {
            try
            {
                var StudentDV = await _context.Estudiante
                    .ToListAsync();

                var StudentDTO = _mapper.Map<List<StudentDTO>>(StudentDV);

                return new Response<List<StudentDTO>>(
                    StudentDTO,
                    "Estudiantes obtenidos correctamente"
                );
            }
            catch (Exception ex)
            {
                return new Response<List<StudentDTO>>(
                    "Error al obtener lista de Estudiantes",
                    new List<string> { ex.Message }
                );
            }
        }

        // Obtener Estudiante por ID
        public async Task<Response<StudentDTO>> GetStudentByIdAsync(Guid id)
        {
            try
            {
                var StudentGT = await _context.Estudiante
                    .Include(x => x.Prestamos)
                    .FirstOrDefaultAsync(x => x.IdEst == Guid.Parse(id.ToString()));

                if (StudentGT == null)
                    return new Response<StudentDTO>("Estudiante no encontrado");

                var StudentDto = _mapper.Map<StudentDTO>(StudentGT);

                return new Response<StudentDTO>(
                    StudentDto,
                    "Estudiante encontrado correctamente"
                );
            }
            catch (Exception ex)
            {
                return new Response<StudentDTO>(
                    "Error al obtener el Estudiante",
                    new List<string> { ex.Message }
                );
            }
        }

        // Crear nuevo Estudiante
        public async Task<Response<StudentDTO>> CreateStudentAsync(StudentDTO StudentDto)
        {
            try
            {

                // Verificar si el usuario ya existe
                var existingUser = await _context.Estudiante
                    .FirstOrDefaultAsync(x => x.IdEst ==StudentDto.IdEst);

                if (existingUser != null)
                    return new Response<StudentDTO>("El Estudiante ya existe");

                // Mapear DTO a modelo
                var students = _mapper.Map<Student>(StudentDto);
                StudentDto.IdEst = Guid.NewGuid();

                // Guardar en base de datos
                _context.Estudiante.Add(students);
                await _context.SaveChangesAsync();

                // Mapear resultado
                var resultDto = _mapper.Map<StudentDTO>(students);

                return new Response<StudentDTO>(
                    resultDto,
                    "Estudiante creado correctamente"
                );
            }
            catch (Exception ex)
            {
                return new Response<StudentDTO>(
                    "Error al crear el Estudiante",
                    new List<string> { ex.Message }
                );
            }
        }

        // Actualizar Estudiante
        public async Task<Response<StudentDTO>> UpdateStudentAsync(Guid id, StudentDTO StudentDto)
        {
            try
            {
                var StudentUP = await _context.Estudiante
                    .Include(x => x.Prestamos)
                    .FirstOrDefaultAsync(x => x.IdEst == Guid.Parse(id.ToString()));

                if (StudentUP == null)
                    return new Response<StudentDTO>("Estudiante no encontrado");

                //// Validar datos

                // Verificar si el Dispositivo ya existe (excepto el actual)
                var existingStu = await _context.Estudiante
                    .FirstOrDefaultAsync(x => x.IdEst == StudentUP.IdEst);

                if (existingStu != null)
                    return new Response<StudentDTO>("El Estudiante ya existe");

                // Actualizar propiedades

                _mapper.Map(StudentDto, StudentUP);

                _context.Estudiante.Update(StudentUP);
                await _context.SaveChangesAsync();

                var resultDto = _mapper.Map<StudentDTO>(StudentUP);

                return new Response<StudentDTO>(
                    resultDto,
                    "Estudiante actualizado correctamente"
                );
            }
            catch (Exception ex)
            {
                return new Response<StudentDTO>(
                    "Error al actualizar el Estudiante",
                    new List<string> { ex.Message }
                );
            }
        }

        // Eliminar Dispositivo
        public async Task<Response<bool>> DeleteStudentAsync(Guid id)
        {
            try
            {
                var StudentDt = await _context.Estudiante
                   .Include(x => x.Prestamos)
                   .FirstOrDefaultAsync(x => x.IdEst == Guid.Parse(id.ToString()));

                if (StudentDt == null)
                    return new Response<bool>("Estudiante no encontrado");

                // Validar si tiene préstamos asociados
                if (StudentDt.Prestamos != null && StudentDt.Prestamos.Any())
                {
                    return new Response<bool>(
                        "No se puede eliminar el Estudiante porque tiene préstamos asociados"
                    );
                }

                _context.Estudiante.Remove(StudentDt);
                await _context.SaveChangesAsync();

                return new Response<bool>(true, "Estudiante eliminado correctamente");
            }
            catch (Exception ex)
            {
                return new Response<bool>(
                    "Error al eliminar el Estudiante",
                    new List<string> { ex.Message }
                );
            }
        }
    }
}
