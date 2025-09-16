using System.Security.Cryptography;
using System.Text.Json;

namespace RubberSearch.Core.Utilities
{
    public class TenantRecord
    {
        public string TenantId { get; set; } = string.Empty;
        public string KeyHash { get; set; } = string.Empty;
        public string KeySalt { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }

    public class ApiKeyMapper
    {
        private readonly string _filePath;
        private readonly object _lock = new();
        private List<TenantRecord> _records = new();

        public ApiKeyMapper(string dataFolder)
        {
            _filePath = Path.Combine(dataFolder, "tenants.json");
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
            Load();
        }

        private void Load()
        {
            lock (_lock)
            {
                if (!File.Exists(_filePath))
                {
                    _records = new List<TenantRecord>();
                    Save();
                    return;
                }
                var json = File.ReadAllText(_filePath);
                var doc = JsonSerializer.Deserialize<Dictionary<string, List<TenantRecord>>>(json)
                          ?? new Dictionary<string, List<TenantRecord>>();
                _records = doc.ContainsKey("tenants") ? doc["tenants"] : new List<TenantRecord>();
            }
        }

        private void Save()
        {
            lock (_lock)
            {
                var tmp = _filePath + ".tmp";
                var json = JsonSerializer.Serialize(new { tenants = _records }, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(tmp, json);
                File.Move(tmp, _filePath, overwrite: true);
            }
        }

        public string CreateTenant()
        {
            var tenantId = "t-" + Convert.ToBase64String(RandomNumberGenerator.GetBytes(6)).Replace('+','-').Replace('/','_');
            var plainKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            var saltBytes = RandomNumberGenerator.GetBytes(16);
            var salt = Convert.ToBase64String(saltBytes);
            using var derive = new Rfc2898DeriveBytes(plainKey, saltBytes, 100_000, HashAlgorithmName.SHA256);
            var hash = Convert.ToBase64String(derive.GetBytes(32));

            var rec = new TenantRecord { TenantId = tenantId, KeyHash = hash, KeySalt = salt, CreatedAt = DateTime.UtcNow, IsActive = true };
            lock (_lock) { _records.Add(rec); Save(); }
            return plainKey;
        }

        public string? GetTenantIdForApiKey(string plainKey)
        {
            lock (_lock)
            {
                foreach (var r in _records.Where(r => r.IsActive))
                {
                    var saltBytes = Convert.FromBase64String(r.KeySalt);
                    using var derive = new Rfc2898DeriveBytes(plainKey, saltBytes, 100_000, HashAlgorithmName.SHA256);
                    var computed = derive.GetBytes(32);
                    var known = Convert.FromBase64String(r.KeyHash);
                    if (CryptographicOperations.FixedTimeEquals(computed, known))
                        return r.TenantId;
                }
            }
            return null;
        }

        public void RevokeTenant(string tenantId)
        {
            lock (_lock)
            {
                var r = _records.FirstOrDefault(x => x.TenantId == tenantId);
                if (r != null) { r.IsActive = false; Save(); }
            }
        }
    }
}