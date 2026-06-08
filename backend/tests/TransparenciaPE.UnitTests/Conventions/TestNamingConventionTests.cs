using System.Reflection;
using System.Text.RegularExpressions;
using FluentAssertions;
using Xunit;

namespace TransparenciaPE.UnitTests.Conventions;

public class TestNamingConventionTests
{
    private static readonly Regex TestMethodNamePattern = new(
        "^[A-Z][A-Za-z0-9]+_[A-Z][A-Za-z0-9]+(?:_(?:When|On|For|With|By)[A-Z][A-Za-z0-9]+)?$",
        RegexOptions.Compiled);

    [Fact]
    public void TestMethods_FollowNamingConvention()
    {
        var invalidTestNames = typeof(TestNamingConventionTests).Assembly
            .GetTypes()
            .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            .Where(method => method.GetCustomAttributes()
                .Any(attribute => attribute is FactAttribute or TheoryAttribute))
            .Select(method => $"{method.DeclaringType!.Name}.{method.Name}")
            .Where(testName => !TestMethodNamePattern.IsMatch(testName.Split('.')[1]))
            .OrderBy(testName => testName)
            .ToList();

        invalidTestNames.Should().BeEmpty(
            "test methods should follow Subject_ExpectedBehavior or Subject_ExpectedBehavior_WhenCondition");
    }
}
