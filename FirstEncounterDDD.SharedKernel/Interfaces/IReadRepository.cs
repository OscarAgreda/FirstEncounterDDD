using Ardalis.Specification;

namespace FirstEncounterDDD.SharedKernel.Interfaces
{

        // <summary>
        /// the reason why we're creating our own interface here is so that we have
        // complete control over it and we can add additional behavior for example in this
        // case we're adding a generic constraint we've said that this particular
        // interface will only work with types that have the I aggregate root interface
        // attached to them or applied to them
        // </summary>
        public interface IReadRepository<T> : IReadRepositoryBase<T> where T : class, IAggregateRoot
        {
        }
}
