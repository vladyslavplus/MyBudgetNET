using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBudget.BLL.DTOs.Expense;
using MyBudget.BLL.Services.Interfaces;
using MyBudget.DAL.Entities.HelpModels;

namespace MyBudget.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ExpenseController : ControllerBase
{
    private readonly IExpenseService _expenseService;

    public ExpenseController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllByUserId(string userId, CancellationToken cancellationToken)
    {
        var expenses = await _expenseService.GetAllByUserIdAsync(userId, cancellationToken);
        return Ok(expenses);
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var expense = await _expenseService.GetByIdAsync(id, cancellationToken);
        return Ok(expense);
    }    
    
    [HttpGet("category/{categoryId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByCategoryId(int categoryId, CancellationToken cancellationToken)
    {
        var expenses = await _expenseService.GetByCategoryIdAsync(categoryId, cancellationToken);
        return Ok(expenses);
    }
    
    [HttpGet("paginated")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPaginated([FromQuery] ExpenseParameters parameters, CancellationToken cancellationToken)
    {
        var paginatedExpenses = await _expenseService.GetPaginatedAsync(parameters, cancellationToken);
        return Ok(paginatedExpenses);
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] ExpenseCreateDto dto, CancellationToken cancellationToken)
    {
        var created = await _expenseService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
    
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int id, [FromBody] ExpenseUpdateDto dto, CancellationToken cancellationToken)
    {
        await _expenseService.UpdateAsync(id, dto, cancellationToken);
        return NoContent();
    }
    
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _expenseService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}