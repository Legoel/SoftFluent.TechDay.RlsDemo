using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Softfluent.Asapp.Core.Data;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Softfluent.Asapp.Core.Settings
{
    public class SettingsManager<TConfigSection, TDbContext> : ISettingsManager<TConfigSection>
                                                               , IDictionary<string, string?>
                                                                where TConfigSection : SettingSectionBase, new()
                                                                where TDbContext : DbContext
    {
        protected Dictionary<string, string?> _agregatedSettings;

        protected Dictionary<string, string?>? _dbSettings;

        protected TConfigSection _section;

        protected IServiceScopeFactory _serviceScopeFactory;

        public SettingsManager(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            _section = new();
            configuration.GetSection(typeof(TConfigSection).Name).Bind(_section, c => c.BindNonPublicProperties = false);

            _serviceScopeFactory = serviceScopeFactory;

            _agregatedSettings = new Dictionary<string, string?>();
            InitializeAggregatedSettings();
        }

        public int Count
        {
            get
            {
                return _agregatedSettings.Count;
            }
        }

        public TConfigSection DefaultSection
        {
            get
            {
                return _section;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return _agregatedSettings.Keys;
            }
        }

        public ICollection<string?> Values
        {
            get
            {
                return _agregatedSettings.Values;
            }
        }

        protected Dictionary<string, string?> DbSettings
        {
            get
            {
                if (_dbSettings == null)
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var repository = scope.ServiceProvider.GetService<IBaseRepository<TDbContext>>();

                        // Retreive all Settings in DB for "*" and ComponentKey
                        string keyToSearch = "," + _section.ComponentKey + ",";
                        var settings = repository?.GetEnumerable<Setting>(x => ("," + x.Targets.Replace(" ", "") + ",").Contains(keyToSearch)
                                                                        || ("," + x.Targets.Replace(" ", "") + ",").Contains(",*,"));
                        if (settings != null)
                            _dbSettings = settings.ToDictionary(x => x.Key, x => x.Value);
                        else
                            _dbSettings = new Dictionary<string, string?>();
                    }
                }
                return _dbSettings;
            }
        }

        public string? this[string key]
        {
            get
            {
                object? propertyValue = typeof(TConfigSection).GetProperty(key)?.GetValue(_section);
                if (propertyValue != null)
                {
                    return propertyValue.ToString();
                }
                else
                {
                    return GetValueFromDatabase(key);
                }
            }
            set
            {
                throw new Exception("Settings Dictionnary Setter is not allowed : use public methods");
            }
        }

        public void Add(string key, string? value)
        {
            throw new Exception("Settings Dictionnary Add is not allowed : use public methods");
        }

        public void Add(KeyValuePair<string, string?> item)
        {
            throw new Exception("Settings Dictionnary Add is not allowed : use public methods");
        }

        public void Clear()
        {
            throw new Exception("Settings Dictionnary Clear is not allowed : use public methods");
        }

        public bool Contains(KeyValuePair<string, string?> item)
        {
            return _agregatedSettings.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return _agregatedSettings.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, string?>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            if ((array.Length - arrayIndex) < Count)
            {
                throw new ArgumentException("The number of elements in internal Settings dictionnary is greater than the available space from arrayIndex", nameof(arrayIndex));
            }

            var settingsEnumerator = _agregatedSettings.GetEnumerator();
            for (int i = arrayIndex; i < array.Length; i++)
            {
                if (settingsEnumerator.MoveNext())
                {
                    array[i] = new KeyValuePair<string, string?>(settingsEnumerator.Current.Key, settingsEnumerator.Current.Value);
                }
            }
        }

        public IEnumerator<KeyValuePair<string, string?>> GetEnumerator()
        {
            return _agregatedSettings.GetEnumerator();
        }

        public long? GetLongValue(string key)
        {
            _agregatedSettings.TryGetValue(key, out string? value);
            return (value != null) ? long.Parse(value) : null;
        }

        public string? GetValueFromDatabase(string key)
        {
            DbSettings.TryGetValue(key, out string? value);
            return value;
        }

        public bool Remove(string key)
        {
            throw new Exception("Settings Dictionnary Remove is not allowed : use public methods");
        }

        public bool Remove(KeyValuePair<string, string?> item)
        {
            throw new Exception("Settings Dictionnary Remove is not allowed : use public methods");
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out string? value)
        {
            return _agregatedSettings.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _agregatedSettings.GetEnumerator();
        }

        protected void InitializeAggregatedSettings()
        {
            // Retreive all Public Properties af AppSettings section
            PropertyInfo[] propertiesList = typeof(TConfigSection).GetProperties();
            foreach (PropertyInfo property in propertiesList)
            {
                _agregatedSettings.Add(property.Name, property.GetValue(_section)?.ToString());
            }

            // Add all DbSettings
            foreach (var kvp in DbSettings)
            {
                _agregatedSettings.Add(kvp.Key, kvp.Value);
            }
        }
    }
}
