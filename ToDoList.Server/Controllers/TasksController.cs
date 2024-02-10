using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoList.Server.Models;

namespace ToDoList.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly DbChallengeNwcWjiF8ielContext _dbcontext;

        public TasksController(DbChallengeNwcWjiF8ielContext context)
        {
            _dbcontext = context;
        }

        [HttpGet]
        [Route("Lista")]
        [Produces("application/json")]
        public async Task<IActionResult> ListaConCategorias()
        {
            try
            {
                var tasksWithCategories = await _dbcontext.Tasks
                    .Include(t => t.Categories)
                    .OrderBy(t => EF.Property<DateTime>(t, "Deadline"))
                    .Select(t => new
                    {
                        TaskId = t.Id,
                        TaskName = t.Name,
                        Completed = t.Completed,
                        Deadline = t.Deadline,
                        CreatedAt = t.CreatedAt,
                        UpdatedAt = t.UpdatedAt,
                        Categories = t.Categories.Select(ct => new
                        {
                            CategoryId = ct.Id,
                            CategoryName = ct.Name
                        }).ToList()
                    })
                    .ToListAsync();

                return Ok(tasksWithCategories);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error en la obtención de la lista: {ex.Message}");
            }
        }





        [HttpPost]
        [Route("Crear")]
        public async Task<IActionResult> Crear([FromBody] Models.Task request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("La tarea proporcionada es nula");
                }




                if (string.IsNullOrEmpty(request.Name))
                {
                    return BadRequest("El nombre de la tarea es obligatorio");
                }

                if (request.CreatedAt == DateTime.MinValue)
                {
                    request.CreatedAt = DateTime.Now;
                }

                await _dbcontext.Tasks.AddAsync(request);
                await _dbcontext.SaveChangesAsync();


                var tasks = await _dbcontext.Tasks.ToListAsync();

                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error en la creación de la tarea: {ex.Message}");
            }
        }


        [HttpPut]
        [Route("Completar/{id:int}")]
        public async Task<IActionResult> Completar(int id)
        {
            try
            {
               
                await _dbcontext.Database.ExecuteSqlInterpolatedAsync($@"
            UPDATE tasks
            SET completed = true, updated_at = CURRENT_TIMESTAMP
            WHERE id = {id}
        ");

               
                var tareaActualizada = await _dbcontext.Tasks
                    .Where(t => t.Id == id)
                    .FirstOrDefaultAsync();

                return Ok(new
                {
                    TaskId = tareaActualizada.Id,
                    TaskName = tareaActualizada.Name,
                    Completed = tareaActualizada.Completed,
                    Deadline = tareaActualizada.Deadline,
                    CreatedAt = tareaActualizada.CreatedAt,
                    UpdatedAt = tareaActualizada.UpdatedAt,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al marcar la tarea como completada: {ex.Message}");
            }
        }







        [HttpPut]
        [Route("EstablecerDeadline/{id:int}")]
        public async Task<IActionResult> EstablecerDeadline(int id, [FromBody] DateTime deadline)
        {
            try
            {
                
                var tareaEnBD = await _dbcontext.Tasks.FindAsync(id);

                if (tareaEnBD == null)
                {
                    return NotFound(); 
                }

               
                tareaEnBD.Deadline = deadline;

                
                await _dbcontext.SaveChangesAsync();

                return Ok($"Deadline de la tarea establecido a: {deadline}");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al establecer el deadline de la tarea: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("Eliminar/{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                
                var tareaEnBD = await _dbcontext.Tasks.FindAsync(id);

                if (tareaEnBD == null)
                {
                    return NotFound(); 
                }

                
                _dbcontext.Tasks.Remove(tareaEnBD);
                await _dbcontext.SaveChangesAsync();

                return Ok("Tarea eliminada exitosamente");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error en la eliminación de la tarea: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("Actualizar/{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Models.Task request)
        {
            try
            {
                
                var tareaEnBD = await _dbcontext.Tasks.FindAsync(id);

                if (tareaEnBD == null)
                {
                    return NotFound(); 
                }

                
                tareaEnBD.Name = request.Name;
                tareaEnBD.Completed = request.Completed;
                tareaEnBD.Deadline = request.Deadline;
                tareaEnBD.Categories = request.Categories;
                tareaEnBD.UpdatedAt = DateTime.Now;

               
                await _dbcontext.SaveChangesAsync();

                return Ok("Tarea actualizada exitosamente");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error en la actualización de la tarea: {ex.Message}");
            }
        }

        

        [HttpGet]
        [Route("ListaCategorias")]
        public async Task<IActionResult> ListaCategorias()
        {
            try
            {
                List<Category> categorias = await _dbcontext.Categories.ToListAsync();
                return Ok(categorias);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error en la obtención de la lista de categorías: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("CrearCategoria")]
        public async Task<IActionResult> CrearCategoria([FromBody] Category request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("La categoría proporcionada es nula");
                }

                if (string.IsNullOrEmpty(request.Name))
                {
                    return BadRequest("El nombre de la categoría es obligatorio");
                }

                await _dbcontext.Categories.AddAsync(request);
                await _dbcontext.SaveChangesAsync();

                return CreatedAtAction(nameof(ListaCategorias), new { id = request.Id }, request);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error en la creación de la categoría: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("ActualizarCategoria/{id:int}")]
        public async Task<IActionResult> ActualizarCategoria(int id, [FromBody] Category request)
        {
            try
            {
               
                var categoriaEnBD = await _dbcontext.Categories.FindAsync(id);

                if (categoriaEnBD == null)
                {
                    return NotFound(); 
                }

                
                categoriaEnBD.Name = request.Name;

                
                await _dbcontext.SaveChangesAsync();

                return Ok("Categoría actualizada exitosamente");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error en la actualización de la categoría: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("EliminarCategoria/{id:int}")]
        public async Task<IActionResult> EliminarCategoria(int id)
        {
            try
            {
               
                var tareasEnCategoria = await _dbcontext.Tasks.AnyAsync(t => t.Id == id);

                if (tareasEnCategoria)
                {
                    return BadRequest("No se puede eliminar la categoría porque hay tareas asociadas");
                }

                
                var categoriaEnBD = await _dbcontext.Categories.FindAsync(id);

                if (categoriaEnBD == null)
                {
                    return NotFound(); 
                }

                
                _dbcontext.Categories.Remove(categoriaEnBD);
                await _dbcontext.SaveChangesAsync();

                return Ok("Categoría eliminada exitosamente");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error en la eliminación de la categoría: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("TareasPorCategoria/{id:int}")]
        public async Task<IActionResult> TareasPorCategoria(int id)
        {
            try
            {
                
                List<Models.Task> tareasPorCategoria = await _dbcontext.Tasks
                    .Where(t => t.Id == id)
                    .OrderByDescending(t => t.Deadline)
                    .ToListAsync();

                return Ok(tareasPorCategoria);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error en la obtención de las tareas por categoría: {ex.Message}");
            }
        }


    }
}
