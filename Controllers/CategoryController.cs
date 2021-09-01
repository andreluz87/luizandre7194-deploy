using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

// Endpoint => URL
// https://localhost:5001
// https://meuapp.azurewebsites.net

[Route("categories")]
// https://meuapp.azurewebsites.net/banana
public class CategoryController : ControllerBase
{

    [HttpGet]
    [Route("")]
    [AllowAnonymous]
    //Método com cache de acordo com: Startup->Configuration->services.AddResponseCaching()
    [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
    //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<ActionResult<List<Category>>> Get([FromServices]DataContext context)
    {
        //AsNoTracking: executa rapidamente a consulta e retorna somente os dados, não preenchendo com informações adicionais do EntityFramework, usado somente como leitura, sem alterações.
        //ToList: Executa a Query de fato ao banco, onde para ser feito orderby, where, sort, fazer antes do ToList.
        var categories = await context.Categories.AsNoTracking().ToListAsync();
        return Ok(categories);
    }

    [HttpGet]
    [Route("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<Category>> GetById(int id,
        [FromServices]DataContext context)
    {
        var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(cat => cat.Id == id);

        if (category == null)
            return NotFound(new { message = "Categoria não encontrada" });
            
        return Ok(category);
    }

    [HttpPost]
    [Route("")]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<List<Category>>> Post(
        [FromBody] Category model,
        [FromServices] DataContext context)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            context.Categories.Add(model);
            await context.SaveChangesAsync();
            return Ok(model);
        }
        catch
        {
            return BadRequest(new { message = "Não foi possível adicionar uma nova categoria" });
        }
    }

    //Envia por parâmetro e por body (Get, Post)
    [HttpPut]
    [Route("{id}")]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<List<Category>>> Put(int id,
        [FromBody] Category model,
        [FromServices] DataContext context)
    {
        if (id != model.Id)
            return NotFound(new { message = "Categoria não encontrada" });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            context.Entry<Category>(model).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return Ok(model);
        }
        //DbUpdateConcurrencyException: Tentou atualizar o registro e ele ja foi atualizado.
        catch (DbUpdateConcurrencyException)
        {
            return BadRequest(new { message = $"Este registro já foi atualizado." });
        }
        catch (Exception)
        {
            return BadRequest(new { message = $"Não foi possível atualizar a categoria id: {id}" });
        }
    }

    [HttpDelete]
    [Route("{id:int}")]
    [Authorize(Roles = "manager")]
    public async Task<ActionResult<List<Category>>> Delete(int id,
        [FromServices] DataContext context)
    {
        var category = await context.Categories.FirstOrDefaultAsync(cat => cat.Id == id);
        if (category == null)
            return NotFound(new { message = "Categoria não encontrada" });

        try
        {
            context.Categories.Remove(category);
            await context.SaveChangesAsync();
            return Ok(new { message = $"A categoria '{category.Title}' foi excluida com sucesso." });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Não foi possível excluir a categoria." });
        }
    }

}