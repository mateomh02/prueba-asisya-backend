using Asisya.Application.Categories;
using Asisya.Application.Categories.Dto;
using Asisya.Application.Categories.Interfaces;
using Asisya.Application.Common.Exceptions;
using Asisya.Domain.Entities;
using NSubstitute;

namespace Asisya.UnitTests.Services;

public class CategoryServiceTests
{
    [Fact]
    public async Task CreateAsync_WhenNameAlreadyExists_ThrowsConflictException()
    {
        var repository = Substitute.For<ICategoryRepository>();
        repository.ExistsByNameAsync("SERVIDORES", Arg.Any<CancellationToken>()).Returns(true);
        var service = new CategoryService(repository);

        await Assert.ThrowsAsync<ConflictException>(() =>
            service.CreateAsync(new CreateCategoryRequest { Name = "SERVIDORES" }));

        await repository.DidNotReceive().AddAsync(Arg.Any<Category>(), Arg.Any<CancellationToken>());
    }
}
