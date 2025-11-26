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
            // AÑADIR .Include() para cargar el objeto de navegación
            .Include(s => s.EstadoEst)
            .ToListAsync();

                var StudentDTO = _mapper.Map<List<StudentDTO>>(StudentDV);

                return Response<List<StudentDTO>>.Success(StudentDTO, "Lista de Estudiantes obtenida correctamente");
            }
            catch (Exception)
            {
                return  Response<List<StudentDTO>>.Failure(
                    "Error al obtener la lista de Estudiantes"
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
                    return Response<StudentDTO>.Failure("Estudiante no encontrado");

                var StudentDto = _mapper.Map<StudentDTO>(StudentGT);

                return Response<StudentDTO>.Success(StudentDto, "Estudiante obtenido correctamente");
            }
            catch (Exception )
            {
                return Response<StudentDTO>.Failure(
                    "Error al obtener el Estudiante"
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
                    return  Response<StudentDTO>.Failure("El Estudiante ya existe");

                var existingCarnet = await _context.Estudiante
                    .FirstOrDefaultAsync(x => x.Carnet == StudentDto.Carnet);

                if (existingUser != null)
                    return Response<StudentDTO>.Failure("El Carnet ya existe, digite otro");


                // Mapear DTO a modelo
                Guid defaultStatusId = Guid.Parse("1EAA1209-075C-4E29-91C9-33824518AD93");
                StudentDto.EstadoEstId = defaultStatusId;
                StudentDto.IdEst = Guid.NewGuid();
                var students = _mapper.Map<Student>(StudentDto);
               

                // Guardar en base de datos
                _context.Estudiante.Add(students);
                await _context.SaveChangesAsync();

                // Mapear resultado
                var resultDto = _mapper.Map<StudentDTO>(students);

                return Response<StudentDTO>.Success(
                    resultDto,
                    "Estudiante creado correctamente"
                );
            }
            catch (Exception)
            {
                return  Response<StudentDTO>.Failure(
                    "Error al crear el Estudiante"
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
                    return Response<StudentDTO>.Failure("Estudiante no encontrado");


                // Actualizar propiedades

                _mapper.Map(StudentDto, StudentUP);

                _context.Estudiante.Update(StudentUP);
                await _context.SaveChangesAsync();

                var resultDto = _mapper.Map<StudentDTO>(StudentUP);

                return Response<StudentDTO>.Success(
                    resultDto,
                    "Estudiante actualizado correctamente"
                );
            }
            catch (Exception)
            {
                return Response<StudentDTO>.Failure(
                    "Error al actualizar el Estudiante"
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
                    return Response<bool>.Failure("Estudiante no encontrado");

                // Validar si tiene préstamos asociados
                if (StudentDt.Prestamos != null && StudentDt.Prestamos.Any())
                {
                    return  Response<bool>.Failure(
                        "No se puede eliminar el Estudiante porque tiene préstamos asociados"
                    );
                }

                _context.Estudiante.Remove(StudentDt);
                await _context.SaveChangesAsync();

                return Response<bool>.Success(true, "Estudiante eliminado correctamente");
            }
            catch (Exception)
            {
                return  Response<bool>.Failure(
                    "Error al eliminar el Estudiante"
                );
            }
        }
    }
}
