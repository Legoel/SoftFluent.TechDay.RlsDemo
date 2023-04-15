namespace Softfluent.Asapp.Core.Data
{
    public class SortColumnAttribute : Attribute
    {
        public SortColumnAttribute(string name, string groupName = "", int order = 0)
        {
            this.Name = name;
            this.GroupName = groupName;
            this.Order = order;
        }

        public string GroupName { get; set; }

        public string Name { get; set; }

        public int? Order { get; set; }
    }
}
