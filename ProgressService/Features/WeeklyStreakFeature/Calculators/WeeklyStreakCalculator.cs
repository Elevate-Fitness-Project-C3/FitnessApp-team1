namespace ProgressService.Features.WeeklyStreakFeature.Calculators
{
    public record StreakResult(int CurrentStreak, int LongestStreak);
    public class WeeklyStreakCalculator
    {
        //1. Monday of the week (in the given time zone) that contains the UTC instant.
        public static DateOnly GetWeekStart(DateTime utc, TimeZoneInfo tz)
        {
            var utcFixed = DateTime.SpecifyKind(utc, DateTimeKind.Utc);
            var local = TimeZoneInfo.ConvertTimeFromUtc(utcFixed, tz);
            var date = DateOnly.FromDateTime(local);
            var daysSinceMonday = ((int)date.DayOfWeek + 6) % 7; // Started Monday = 0 - Sunday = 6
            return date.AddDays(-daysSinceMonday);
        }

        //2.
        public static StreakResult Calculate(IEnumerable<DateTime> completedAtUtc,DateTime nowUtc,TimeZoneInfo tz)
        {
            //1. Unique active weeks, sorted ascending :
            var weeks = completedAtUtc
                .Select(d=> GetWeekStart(d, tz))
                .Distinct()
                .OrderBy(W=>W)
                .ToList();
            
            //2.if the user has not completed any weeks, return 0,0
            if(weeks.Count == 0)
                return new StreakResult(0, 0);

            //3.Longest: max run of consecutive weeks (7 days apart)
            int longest = 1, run = 1;
            for (int i = 1; i < weeks.Count; i++)
            {
                run = weeks[i].DayNumber - weeks[i - 1].DayNumber == 7 ? run + 1 : 1;
                longest = Math.Max(longest, run);
            }

            //4.Current: walk back from the current week (or the previous one if current is not active yet)
            var set = weeks.ToHashSet(); 
            var currentWeek = GetWeekStart(nowUtc, tz);
            var cursor = set.Contains(currentWeek) ? currentWeek : currentWeek.AddDays(-7);

            int current = 0;
            while (set.Contains(cursor))
            {
                current++;
                cursor = cursor.AddDays(-7);
            }

            return new StreakResult(current, longest);

        }
    }
}
