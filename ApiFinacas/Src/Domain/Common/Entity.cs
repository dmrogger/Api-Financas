namespace ApiFinancas.Src.Domain.Common
{
    /// <summary>
    /// Classe base para todas as entidades do domínio
    /// Fornece propriedades comuns como Id e timestamps
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// Identificador único da entidade
        /// </summary>
        public Guid Id { get; protected set; }

        /// <summary>
        /// Data de criação da entidade
        /// </summary>
        public DateTime DataCriacao { get; protected set; }

        /// <summary>
        /// Data da última atualização da entidade
        /// </summary>
        public DateTime? DataUltimaAtualizacao { get; protected set; }

        /// <summary>
        /// Construtor protegido para forçar uso de herança
        /// </summary>
        protected Entity()
        {
            Id = Guid.NewGuid();
            DataCriacao = DateTime.UtcNow;
        }

        /// <summary>
        /// Construtor protegido com ID específico (útil para reconstrução de entidades)
        /// </summary>
        protected Entity(Guid id)
        {
            Id = id;
            DataCriacao = DateTime.UtcNow;
        }

        /// <summary>
        /// Marca a entidade como atualizada
        /// </summary>
        protected void MarcarComoAtualizada()
        {
            DataUltimaAtualizacao = DateTime.UtcNow;
        }

        /// <summary>
        /// Verifica se duas entidades são iguais baseado no Id
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is not Entity other)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;

            if (Id == Guid.Empty || other.Id == Guid.Empty)
                return false;

            return Id == other.Id;
        }

        /// <summary>
        /// Retorna o hash code baseado no Id
        /// </summary>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <summary>
        /// Operador de igualdade
        /// </summary>
        public static bool operator ==(Entity? left, Entity? right)
        {
            if (left is null && right is null)
                return true;

            if (left is null || right is null)
                return false;

            return left.Equals(right);
        }

        /// <summary>
        /// Operador de desigualdade
        /// </summary>
        public static bool operator !=(Entity? left, Entity? right)
        {
            return !(left == right);
        }
    }
}
