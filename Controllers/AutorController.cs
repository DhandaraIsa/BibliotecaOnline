using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BibliotecaOnline.Data;
using BibliotecaOnline.Models;

namespace BibliotecaOnline.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutorController : ControllerBase
    {
        private readonly BibliotecaContext _context;
        private readonly ILogger<AutorController> _logger;

        public AutorController(BibliotecaContext context, ILogger<AutorController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Autor>>> GetAutores()
        {
            try
            {
                var autores = await _context.Autores
                    .Include(a => a.Livros)
                    .ToListAsync();
                return Ok(autores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar autores");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Autor>> GetAutor(int id)
        {
            try
            {
                var autor = await _context.Autores
                    .Include(a => a.Livros)
                    .FirstOrDefaultAsync(a => a.Id == id);
                
                if (autor == null)
                {
                    return NotFound($"Autor com ID {id} não encontrado");
                }
                
                return Ok(autor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar autor com ID {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("{id}/livros")]
        public async Task<ActionResult<IEnumerable<Livro>>> GetLivrosPorAutor(int id)
        {
            try
            {
                var autor = await _context.Autores
                    .Include(a => a.Livros)
                    .FirstOrDefaultAsync(a => a.Id == id);
                
                if (autor == null)
                {
                    return NotFound($"Autor com ID {id} não encontrado");
                }
                
                return Ok(autor.Livros);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar livros do autor com ID {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Autor>> PostAutor(Autor autor)
        {
            try
            {
                // Validação do modelo
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validação de negócio
                if (string.IsNullOrWhiteSpace(autor.Nome))
                {
                    return BadRequest("Nome do autor é obrigatório");
                }

                if (autor.Nome.Length < 2)
                {
                    return BadRequest("Nome do autor deve ter pelo menos 2 caracteres");
                }

                if (autor.Nome.Length > 100)
                {
                    return BadRequest("Nome do autor não pode exceder 100 caracteres");
                }

                // Verificar se já existe um autor com o mesmo nome
                var autorExistente = await _context.Autores
                    .FirstOrDefaultAsync(a => a.Nome.ToLower().Trim() == autor.Nome.ToLower().Trim());
                
                if (autorExistente != null)
                {
                    return BadRequest($"Já existe um autor com o nome '{autor.Nome}'");
                }

                _context.Autores.Add(autor);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Autor '{Nome}' criado com ID {Id}", autor.Nome, autor.Id);
                
                return CreatedAtAction(nameof(GetAutor), new { id = autor.Id }, autor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar autor");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAutor(int id, Autor autor)
        {
            try
            {
                if (id != autor.Id)
                {
                    return BadRequest("ID da URL não corresponde ao ID do autor");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var autorExistente = await _context.Autores.FindAsync(id);
                if (autorExistente == null)
                {
                    return NotFound($"Autor com ID {id} não encontrado");
                }

                // Validações de negócio
                if (string.IsNullOrWhiteSpace(autor.Nome))
                {
                    return BadRequest("Nome do autor é obrigatório");
                }

                if (autor.Nome.Length < 2)
                {
                    return BadRequest("Nome do autor deve ter pelo menos 2 caracteres");
                }

                if (autor.Nome.Length > 100)
                {
                    return BadRequest("Nome do autor não pode exceder 100 caracteres");
                }

                // Verificar se já existe outro autor com o mesmo nome (excluindo o atual)
                var autorComMesmoNome = await _context.Autores
                    .FirstOrDefaultAsync(a => a.Id != id && 
                                            a.Nome.ToLower().Trim() == autor.Nome.ToLower().Trim());
                
                if (autorComMesmoNome != null)
                {
                    return BadRequest($"Já existe outro autor com o nome '{autor.Nome}'");
                }

                autorExistente.Nome = autor.Nome.Trim();

                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Autor com ID {Id} atualizado para '{Nome}'", id, autor.Nome);
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar autor com ID {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAutor(int id)
        {
            try
            {
                var autor = await _context.Autores
                    .Include(a => a.Livros)
                    .FirstOrDefaultAsync(a => a.Id == id);
                
                if (autor == null)
                {
                    return NotFound($"Autor com ID {id} não encontrado");
                }

                // Verificar se o autor tem livros associados
                if (autor.Livros.Any())
                {
                    return BadRequest($"Não é possível deletar o autor '{autor.Nome}' pois possui {autor.Livros.Count} livro(s) associado(s). Remova os livros primeiro ou use a funcionalidade de desativação.");
                }

                _context.Autores.Remove(autor);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Autor '{Nome}' com ID {Id} deletado", autor.Nome, id);
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar autor com ID {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<Autor>>> BuscarAutores([FromQuery] string? nome)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nome))
                {
                    return await GetAutores();
                }

                var autores = await _context.Autores
                    .Include(a => a.Livros)
                    .Where(a => a.Nome.ToLower().Contains(nome.ToLower()))
                    .ToListAsync();

                return Ok(autores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar autores com nome '{Nome}'", nome);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("estatisticas")]
        public async Task<ActionResult<object>> GetEstatisticasAutores()
        {
            try
            {
                var estatisticas = await _context.Autores
                    .Select(a => new
                    {
                        a.Id,
                        a.Nome,
                        TotalLivros = a.Livros.Count,
                        LivrosDisponiveis = a.Livros.Count(l => l.Disponivel),
                        LivrosEmprestados = a.Livros.Count(l => !l.Disponivel)
                    })
                    .OrderByDescending(a => a.TotalLivros)
                    .ToListAsync();

                return Ok(estatisticas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar estatísticas dos autores");
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }
}