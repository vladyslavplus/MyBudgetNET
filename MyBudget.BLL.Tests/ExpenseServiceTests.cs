using FakeItEasy;
using FluentAssertions;
using MyBudget.BLL.Services;
using MyBudget.BLL.Services.Interfaces;
using MyBudget.DAL.Entities;
using MyBudget.DAL.Repositories.Interfaces;
using MyBudget.DAL.UOW;

namespace MyBudget.BLL.Tests;

public class ExpenseServiceTests
{
    private readonly CancellationToken _ct = CancellationToken.None;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IExpenseRepository _expenseRepository;
    private readonly IExpenseService _expenseService;
    
    public ExpenseServiceTests()
    {
        _unitOfWork = A.Fake<IUnitOfWork>();
        _expenseRepository = A.Fake<IExpenseRepository>();
        A.CallTo(() => _unitOfWork.Expenses).Returns(_expenseRepository);
        _expenseService = new ExpenseService(_unitOfWork);
    }

}