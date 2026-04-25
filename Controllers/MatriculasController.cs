using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab06_AlexandroCano.Data;
using Lab06_AlexandroCano.Models;
using Lab06_AlexandroCano.DTOs;

namespace Lab06_AlexandroCano.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Todo el controlador requiere JWT válido
    public class MatriculasController : ControllerBase
    {
        private readonly Lab05MatriculasDbContext _context;

        public MatriculasController(Lab05MatriculasDbContext context)
        {
            _context = context;
        }

        // GET: api/matriculas - cualquier usuario autenticado
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var matriculas = await _context.Matriculas
                .Select(m => new
                {
                    m.IdMatricula,
                    m.IdEstudiante,
                    m.IdCurso,
                    m.IdProfesor,
                    m.Semestre
                })
                .ToListAsync();

            return Ok(matriculas);
        }

        // GET: api/matriculas/5 - User o Admin
        [HttpGet("{id}")]
        [Authorize(Policy = "UserOrAdmin")]
        public async Task<IActionResult> GetById(int id)
        {
            var matricula = await _context.Matriculas.FindAsync(id);
            if (matricula == null)
                return NotFound(new { message = $"Matrícula {id} no encontrada." });

            return Ok(matricula);
        }

        // POST: api/matriculas - solo Admin
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create([FromBody] MatriculaCreateDto dto)
        {
            var matricula = new Matricula
            {
                IdEstudiante = dto.IdEstudiante,
                IdCurso = dto.IdCurso,
                IdProfesor = dto.IdProfesor,
                Semestre = dto.Semestre
            };

            _context.Matriculas.Add(matricula);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = matricula.IdMatricula }, new
            {
                matricula.IdMatricula,
                matricula.IdEstudiante,
                matricula.IdCurso,
                matricula.IdProfesor,
                matricula.Semestre
            });
        }
        // DELETE: api/matriculas/5 - solo Admin
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var matricula = await _context.Matriculas.FindAsync(id);
            if (matricula == null)
                return NotFound();

            _context.Matriculas.Remove(matricula);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"Matrícula {id} eliminada." });
        }
    }
}