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

                return  Response<List<studentStatusDTO>>.Success(
                    StudentDTO,
                    "Estado del estudiante obtenidos correctamente"
                );
            }
            catch (Exception)
            {
                return  Response<List<studentStatusDTO>>.Failure(
                    "Error al obtener lista del estado de los Estudiantes"
                );
            }
        }

        // Obtener Estudiante por ID
        public async Task<Response<studentStatusDTO>> GetStudentStaByIdAsync(Guid id)
        {
            try
            {
                var StudentStaGT = await _context.EstadoEstudiantes
                    // .Include(x => x.Prestamos)
                    .FirstOrDefaultAsync(x => x.IdStatus == Guid.Parse(id.ToString()));

                if (StudentStaGT == null)
                    return Response<studentStatusDTO>.Failure("Estado del Estudiante no encontrado");

                var StudentStaDto = _mapper.Map<studentStatusDTO>(StudentStaGT);

                return Response<studentStatusDTO>.Success(
                    StudentStaDto,
                    "Estado del Estudiante encontrado correctamente"
                );
            }
            catch (Exception )
            {
                return  Response<studentStatusDTO>.Failure(
                    "Error al obtener el Estado del Estudiante"
                   
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
                    return Response<studentStatusDTO>.Failure("El estado del estudiante ya existe");

                // Mapear DTO a modelo
               
                StudentStaDto.IdStatus = Guid.NewGuid();
                var studentsSta = _mapper.Map<studentStatus>(StudentStaDto);

                // Guardar en base de datos
                _context.EstadoEstudiantes.Add(studentsSta);
                await _context.SaveChangesAsync();

                // Mapear resultado
                var resultDto = _mapper.Map<studentStatusDTO>(studentsSta);

                return Response<studentStatusDTO>.Success(
                    resultDto,
                    "Estado del Estudiante creado correctamente"
                );
            }
            catch (Exception)
            {
                return Response<studentStatusDTO>.Failure(
                    "Error al crear el estado del estudiante"
                    
                );
            }
        }

        // Actualizar Estudiante
        public async Task<Response<studentStatusDTO>> UpdateStudentStaAsync(Guid id, studentStatusDTO StudentDto)
        {
            try
            {
                var StudentUP = await _context.EstadoEstudiantes
                    // .Include(x => x.Prestamos)
                    .FirstOrDefaultAsync(x => x.IdStatus == Guid.Parse(id.ToString()));

                if (StudentUP == null)
                    return  Response<studentStatusDTO>.Failure("estado del estudiante no encontrado");



                // Actualizar propiedades

                _mapper.Map(StudentDto, StudentUP);

                _context.EstadoEstudiantes.Update(StudentUP);
                await _context.SaveChangesAsync();

                var resultDto = _mapper.Map<studentStatusDTO>(StudentUP);

                return  Response<studentStatusDTO>.Success(
                    resultDto,
                    "Estado del estudiante actualizado correctamente"
                );
            }
            catch (Exception)
            {
                return  Response<studentStatusDTO>.Failure(
                    "Error al actualizar el Estudiante"
                     
                );
            }
        }

        // Eliminar Dispositivo
        public async Task<Response<bool>> DeleteStudentStatusAsync(Guid id)
        {
            try
            {
                // Buscar el estado del estudiante por ID, incluyendo su relación con Student
                var StudentStaDt = await _context.EstadoEstudiantes
                    .Include(x => x.studentsStu)
                    .FirstOrDefaultAsync(x => x.IdStatus == id);

                // Validar si existe
                if (StudentStaDt == null)
                    return  Response<bool>.Failure("Estado del estudiante no encontrado");

                // Validar si está asociado a un estudiante
                if (StudentStaDt.studentsStu != null)
                {
                    return  Response<bool>.Failure(
                        "No se puede eliminar el estado del estudiante porque está asociado a un estudiante."
                    );
                }

                // Eliminar el registro
                _context.EstadoEstudiantes.Remove(StudentStaDt);
                await _context.SaveChangesAsync();

                return  Response<bool>.Success(true, "Estado del estudiante eliminado correctamente.");
            }
            catch (Exception)
            {
                return  Response<bool>.Failure(
                    "Error al eliminar el estado del estudiante"
                   
                );
            }
        }
    }
}