using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWatch.Logger
{
    public interface ICustomLogger
    {
        Task LogInfoAsync(string message, object? extraData = null);
        Task LogWarningAsync(string message, object? extraData = null);
        Task LogErrorAsync(string message, Exception? exception = null, object? extraData = null);
    }
}
