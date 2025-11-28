using ApiFinaças.Src.Domain.Common;

namespace ApiFinaças.Src.Domain.Common
{
    /// <summary>
    /// Classe base para Aggregate Roots
    /// Herda de Entity e adiciona funcionalidades específicas de Aggregate Root
    /// </summary>
    public abstract class AggregateRoot : Entity
    {
        /// <summary>
        /// Construtor protegido que chama o construtor da classe base Entity
        /// </summary>
        protected AggregateRoot() : base()
        {
        }

        /// <summary>
        /// Construtor protegido com ID específico
        /// </summary>
        protected AggregateRoot(Guid id) : base(id)
        {
        }
    }
}
