using Microsoft.EntityFrameworkCore;
using TransparenciaPE.Infrastructure.Data;

namespace TransparenciaPE.UnitTests.Infrastructure;

internal static class InMemoryDbContextFactory
{
    public static AppDbContext Create()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}
