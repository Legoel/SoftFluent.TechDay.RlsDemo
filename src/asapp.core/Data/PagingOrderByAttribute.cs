namespace Softfluent.Asapp.Core.Data
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class PagingOrderByAttribute : Attribute
    {
        public PagingOrderByAttribute()
            : this(0)
        {
        }

        public PagingOrderByAttribute(int order)
        {
            this.Order = order;
        }

        /// <summary>
        /// Identification of the Type of the Command
        /// </summary>
        public int Order { get; private set; }
    }
}
