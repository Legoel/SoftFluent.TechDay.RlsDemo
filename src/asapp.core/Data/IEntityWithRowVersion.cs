namespace Softfluent.Asapp.Core.Data
{
    /// <summary>
    /// Minimal representation of en Entity. Any managed Entities must derivate from Entity abstract class
    /// </summary>
    /// <typeparam name="TKey">Type of the Identity Key</typeparam>
    public interface IEntityWithRowVersion<TKey> : IEntityWithRowVersion
    {
        /// <summary>
        /// Unique Id
        /// </summary>
        TKey Identifier { get; set; }
    }

    public interface IEntityWithRowVersion : IEntity
    {
        byte[] RowVersion { get; set; }
    }
}
