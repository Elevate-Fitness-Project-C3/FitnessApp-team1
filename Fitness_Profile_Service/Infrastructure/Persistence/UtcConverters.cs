using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FitnessProfileService.Infrastructure.Persistence;

internal static class UtcConverters
{
    public static readonly ValueConverter<DateTime, DateTime> Required =
        new(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

    public static readonly ValueConverter<DateTime?, DateTime?> Optional =
        new(v => v, v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null);
}
