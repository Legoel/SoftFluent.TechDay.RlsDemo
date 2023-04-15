using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Softfluent.Asapp.Core.Data
{
    public static class ModelBuilderExtension
    {
        public static void RegisterAllViews(this ModelBuilder modelBuilder)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            IEnumerable<Type> viewsType = AppDomain.CurrentDomain.GetAssemblies()
                       .SelectMany(t => t.GetTypes()).Where(x => x.GetInterface(nameof(IEntityView)) != null);

            foreach (Type viewType in viewsType)
            {
                modelBuilder.RegisterView(viewType);
            }
        }

        public static void RegisterView(this ModelBuilder modelBuilder, Type viewType)
        {
            PropertyInfo? pi = viewType.GetProperty(nameof(IEntityView.Request), BindingFlags.Public | BindingFlags.Static);
            if (pi != null)
            {
                string? request = (string?)pi.GetValue(null);
                if (!string.IsNullOrEmpty(request))
                {
                    modelBuilder.Entity(viewType).ToSqlQuery(request);
                }
            }
        }

        public static void RegisterView<IEntityView>(this ModelBuilder modelBuilder) where IEntityView : class
        {
            modelBuilder.RegisterView(typeof(IEntityView));
        }

        public static void RegisterViews(this ModelBuilder modelBuilder, string ns)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            IEnumerable<Type> viewsType = AppDomain.CurrentDomain.GetAssemblies()
                       .SelectMany(t => t.GetTypes()).Where(x => x.GetInterface(nameof(IEntityView)) != null
                                                            && x.Namespace == ns);

            foreach (Type viewType in viewsType)
            {
                modelBuilder.RegisterView(viewType);
            }
        }
    }
}
