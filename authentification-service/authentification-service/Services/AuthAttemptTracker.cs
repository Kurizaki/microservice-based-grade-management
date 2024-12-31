using System.Collections.Concurrent;
using Serilog;

namespace authentification_service.Services
{
    public class AuthAttemptTracker
    {
        private readonly ConcurrentDictionary<string, (int Count, DateTime LastAttempt)> _attempts = new();
        private const int MaxAttempts = 3;
        private static readonly TimeSpan ResetTime = TimeSpan.FromMinutes(15);

        public bool ShouldLogAttempt(string username)
        {
            var now = DateTime.UtcNow;
            var attempt = _attempts.AddOrUpdate(
                username,
                (1, now),
                (_, old) =>
                {
                    if (now - old.LastAttempt > ResetTime)
                        return (1, now);
                    return (old.Count + 1, now);
                });

            if (attempt.Count >= MaxAttempts)
            {
                Log.Warning("[{Timestamp:yyyy-MM-dd HH:mm:ss}] WARNING auth.security username={Username} attempts={Attempts} message=\"Multiple failed login attempts detected\"",
                    DateTime.UtcNow, username, attempt.Count);
                return true;
            }

            return false;
        }

        public void ResetAttempts(string username)
        {
            _attempts.TryRemove(username, out _);
        }
    }
}
