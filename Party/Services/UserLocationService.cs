using System.Collections.Concurrent;
namespace Party.Services
{
    public class UserLocationService
    {
        private readonly ConcurrentDictionary<string, string> _userLocations = new();

        // 設定使用者的位置
        public void SetUserLocation(string userId, string location)
        {
            _userLocations[userId] = location;
        }

        // 獲取使用者的位置
        public string GetUserLocation(string userId)
        {
            _userLocations.TryGetValue(userId, out var location);
            return location ?? "Unknown";
        }

        // 刪除使用者的位置（例如，使用者登出時）
        public void RemoveUserLocation(string userId)
        {
            _userLocations.TryRemove(userId, out _);
        }

        // 獲取所有使用者的位置
        public Dictionary<string, string> GetAllUserLocations()
        {
            return new Dictionary<string, string>(_userLocations);
        }
    }


}
