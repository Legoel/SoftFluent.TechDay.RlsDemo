namespace Softfluent.Asapp.Core.Data
{
    ///<inheritdoc cref="IEntity<TKey>"/>
    public abstract class Entity<TKey> : IEntity<TKey> where TKey : notnull
    {
        [PagingOrderBy]
#pragma warning disable CS8601 // Possible null reference assignment.
        public TKey Identifier { get; set; } = default(TKey);

#pragma warning restore CS8601 // Possible null reference assignment.

        public string TrackCreationUser { get; set; } = string.Empty;

        public DateTime TrackCreationTime { get; set; }

        public string TrackLastWriteUser { get; set; } = string.Empty;

        public DateTime TrackLastWriteTime { get; set; }
    }
}
