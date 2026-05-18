using System;
using System.Linq.Expressions;

namespace ClinicalPatientManagement.Api.Specifications
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> Criteria { get; }
    }
}
