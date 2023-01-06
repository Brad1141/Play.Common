// Entites: classes used by the item repository to store data

namespace Play.Common
{
    public interface IEntity
    {
        Guid Id { get; set; }
    }
}