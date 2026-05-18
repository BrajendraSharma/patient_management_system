using System;
using System.Linq.Expressions;
using ClinicalPatientManagement.Api.Specifications;
using Xunit;

public class SpecificationTests
{
    [Fact]
    public void Specification_Criteria_ReturnsCorrectExpression()
    {
        Expression<Func<int, bool>> expr = x => x > 5;
        var spec = new Specification<int>(expr);
        Assert.Equal(expr, spec.Criteria);
    }
}
