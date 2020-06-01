using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiNetCore.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;

namespace ApiNetCore.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class PessoasController : ControllerBase
    {
        private readonly PessoaContext _context;

        public PessoasController(PessoaContext context)
        {
            _context = context;
        }

        // GET: api/Pessoas
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pessoa>>> GetPessoas()
        {
            return await _context.Pessoas.ToListAsync();
        }

        // GET: api/Pessoas/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Pessoa>> GetPessoa(Guid id)
        {
            var pessoa = await _context.Pessoas.FindAsync(id);

            if (pessoa == null)
            {
                return NotFound();
            }

            return pessoa;
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPessoa(Guid id, Pessoa pessoa)
        {
            if (id != pessoa.ID)
            {
                return BadRequest();
            }

            _context.Entry(pessoa).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PessoaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Pessoa>> PostPessoa(Pessoa pessoa)
        {
            _context.Pessoas.Add(pessoa);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPessoa", new { id = pessoa.ID }, pessoa);
        }

        // DELETE: api/Pessoas/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Pessoa>> DeletePessoa(Guid id)
        {
            var pessoa = await _context.Pessoas.FindAsync(id);
            if (pessoa == null)
            {
                return NotFound();
            }

            _context.Pessoas.Remove(pessoa);
            await _context.SaveChangesAsync();

            return pessoa;
        }

        private bool PessoaExists(Guid id)
        {
            return _context.Pessoas.Any(e => e.ID == id);
        }

        [Authorize]
        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(Guid id, [FromBody] JsonPatchDocument<Pessoa> patchPessoa)
        {
            if (patchPessoa == null)
            {
                return BadRequest();
            }
            var pessoaDB = await _context.Pessoas.FirstOrDefaultAsync(p => p.ID == id);
            if (pessoaDB == null)
            {
                return NotFound();
            }
            patchPessoa.ApplyTo(pessoaDB, ModelState);
            var isValid = TryValidateModel(pessoaDB);
            if (!isValid)
            {
                return BadRequest(ModelState);
            }
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }   
}
