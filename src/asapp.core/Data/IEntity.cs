namespace Softfluent.Asapp.Core.Data
{
    /// <summary>
    /// Minimal representation of en Entity. Any managed Entities must derivate from Entity abstract class
    /// </summary>
    /// <typeparam name="TKey">Type of the Identity Key</typeparam>
    public interface IEntity<TKey> : IEntity
    {
        /// <summary>
        /// Unique Id
        /// </summary>
        TKey Identifier { get; set; }
    }

    public interface IEntity
    {
        /// <summary>
        /// Creation Date
        /// </summary>
        DateTime TrackCreationTime { get; set; }

        /// <summary>
        /// Name of user
        /// </summary>
        string TrackCreationUser { get; set; }

        /// <summary>
        /// Update Date
        /// </summary>
        DateTime TrackLastWriteTime { get; set; }

        /// <summary>
        /// Name of user
        /// </summary>
        string TrackLastWriteUser { get; set; }
    }
}
