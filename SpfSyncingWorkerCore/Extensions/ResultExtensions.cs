using Ardalis.Result;

namespace SpfSyncingWorkerCore.Extensions;

public static class ResultExtensions
{
    public static bool Failure(this Result result) { return !result.IsSuccess; }

    public static string ToErrorString(this IEnumerable<string> values)
    {
        return string.Join(";", values);
    }
}
