using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using UsersApi.Data;
using UsersApi.Models;
using UsersApi.DTOs;
using Microsoft.AspNetCore.Identity;

namespace UsersApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST /usuarios
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<Usuario>> CreateUser(CreateUserDto createUserDto)
        {
            // Verificar si el correo ya existe
            if (await _context.Usuarios.AnyAsync(u => u.Correo == createUserDto.Correo))
            {
                return BadRequest("El correo electrónico ya está registrado");
            }
            var hasher = new PasswordHasher<string>();

            // Crear nuevo usuario
            var usuario = new Usuario
            {
                Nombre = createUserDto.Nombre,
                Correo = createUserDto.Correo,
                Contraseña = hasher.HashPassword(null, createUserDto.password)
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = usuario.Id }, usuario);
        }

        // GET /usuarios?page=1&pageSize=10
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<PaginatedResponse<Usuario>>> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var totalItems = await _context.Usuarios.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var usuarios = await _context.Usuarios
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Eliminar la contraseña de la respuesta 
            foreach (var usuario in usuarios)
            {
                usuario.Contraseña = null;
            }

            var response = new PaginatedResponse<Usuario>
            {
                Items = usuarios,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            return Ok(response);
        }

        // GET /usuarios/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Usuario>> GetUser(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            // Eliminar la contraseña de la respuesta por seguridad
            usuario.Contraseña = null;

            return usuario;
        }

        // PUT /usuarios/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDto updateUserDto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            var hasher = new PasswordHasher<string>();
            if (usuario == null)
            {
                return NotFound();
            }

            // Verificar si el correo ya existe (si se está actualizando)
            if (!string.IsNullOrEmpty(updateUserDto.Correo) &&
                updateUserDto.Correo != usuario.Correo &&
                await _context.Usuarios.AnyAsync(u => u.Correo == updateUserDto.Correo))
            {
                return BadRequest("El correo electrónico ya está registrado por otro usuario");
            }

            // Actualizar los campos
            if (!string.IsNullOrEmpty(updateUserDto.Nombre))
                usuario.Nombre = updateUserDto.Nombre;

            if (!string.IsNullOrEmpty(updateUserDto.Correo))
                usuario.Correo = updateUserDto.Correo;

            if (!string.IsNullOrEmpty(updateUserDto.password))
                usuario.Contraseña = hasher.HashPassword(null, updateUserDto.password);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE /usuarios/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}