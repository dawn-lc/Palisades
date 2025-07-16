using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Palisades.Helpers
{
    public static class Extensions
    {
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> debounceTokens = new();

        public static void Debounce(int delayMs, Func<Task> action,string? key = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            key ??= $"{filePath}:{memberName}:{lineNumber}";

            if (debounceTokens.TryGetValue(key, out var oldCts))
            {
                oldCts.Cancel();
                oldCts.Dispose();
            }

            var cts = new CancellationTokenSource();
            debounceTokens[key] = cts;

            var token = cts.Token;

            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(delayMs, token);
                    if (!token.IsCancellationRequested)
                    {
                        await action();
                    }
                }
                catch (TaskCanceledException)
                {
                    // 忽略
                }
                finally
                {
                    debounceTokens.TryRemove(key, out _);
                }
            }, token);
        }
    }
}
