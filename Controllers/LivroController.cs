using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BibliotecaOnline.Data;
using BibliotecaOnline.Models;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaOnline.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LivroController : ControllerBase
    {
        private readonly BibliotecaContext _context;
        private readonly ILogger<LivroController> _logger;

        public LivroController(BibliotecaContext context, ILogger<LivroController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Livro>>> GetLivros()
        {
            try
            {
                var livros = await _context.Livros
                    .Include(l => l.Autor)
                    .ToListAsync();
                return Ok(livros);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar livros");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Livro>> GetLivro(int id)
        {
            try
            {
                var livro = await _context.Livros
                    .Include(l => l.Autor)
                    .FirstOrDefaultAsync(l => l.Id == id);
                
                if (livro == null)
                {
                    return NotFound($"Livro com ID {id} não encontrado");
                }
                
                return Ok(livro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar livro com ID {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Livro>> PostLivro(Livro livro)
        {
            try
            {
                // Validação do modelo
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validação de negócio
                if (livro.AnoPublicacao > DateTime.Now.Year)
                {
                    return BadRequest("Ano de publicação não pode ser futuro");
                }

                // Verificar se o autor existe
                var autorExiste = await _context.Autores.AnyAsync(a => a.Id == livro.AutorId);
                if (!autorExiste)
                {
                    return BadRequest($"Autor com ID {livro.AutorId} não encontrado");
                }

                _context.Livros.Add(livro);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Livro '{Titulo}' criado com ID {Id}", livro.Titulo, livro.Id);
                
                return CreatedAtAction(nameof(GetLivro), new { id = livro.Id }, livro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar livro");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutLivro(int id, Livro livro)
        {
            try
            {
                if (id != livro.Id)
                {
                    return BadRequest("ID da URL não corresponde ao ID do livro");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var livroExistente = await _context.Livros.FindAsync(id);
                if (livroExistente == null)
                {
                    return NotFound($"Livro com ID {id} não encontrado");
                }

                // Verificar se o autor existe
                var autorExiste = await _context.Autores.AnyAsync(a => a.Id == livro.AutorId);
                if (!autorExiste)
                {
                    return BadRequest($"Autor com ID {livro.AutorId} não encontrado");
                }

                livroExistente.Titulo = livro.Titulo;
                livroExistente.AnoPublicacao = livro.AnoPublicacao;
                livroExistente.Disponivel = livro.Disponivel;
                livroExistente.AutorId = livro.AutorId;

                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Livro com ID {Id} atualizado", id);
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar livro com ID {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLivro(int id)
        {
            try
            {
                var livro = await _context.Livros.FindAsync(id);
                if (livro == null)
                {
                    return NotFound($"Livro com ID {id} não encontrado");
                }

                // Verificar se o livro pode ser deletado (não tem empréstimos ativos)
                var temEmprestimos = await _context.Emprestimos
                    .AnyAsync(e => e.LivroId == id && e.DataDevolucao == null);
                
                if (temEmprestimos)
                {
                    return BadRequest("Não é possível deletar um livro com empréstimos ativos");
                }

                _context.Livros.Remove(livro);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Livro com ID {Id} deletado", id);
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar livro com ID {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }
}
