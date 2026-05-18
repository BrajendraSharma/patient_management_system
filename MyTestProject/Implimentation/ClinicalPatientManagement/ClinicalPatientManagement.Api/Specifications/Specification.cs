using System;
using System.Linq.Expressions;

namespace ClinicalPatientManagement.Api.Specifications
{
    public class Specification<T> : ISpecification<T>
    {
        public Specification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }
        public Expression<Func<T, bool>> Criteria { get; }
    }
}
