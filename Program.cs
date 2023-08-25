
using ChessDotComRatings.Models;
using static System.Console;

namespace ChessDotComRatings
{

    public class Program
    {
        static void Main(string[] args)
        {
            var parameters = GetParameters(args);
            RatingsStats stats = Ratings.GetStats(parameters).Result;
            WriteStats(stats);
        }

        static RatingsParameters GetParameters(string[] args)
        {
            var result = new RatingsParameters();
            foreach (var arg in args)
            {
                {
                    int index = arg.IndexOf("=");
                    int index2;
                    string paramValue;
                    int year;
                    int month;
                    var paramName = arg.Substring(0, index).ToLower();
                    switch (paramName)
                    {
                        case "user":
                            result.UserName = arg.Substring(index + 1);
                            break;
                        case "start":
                            paramValue = arg.Substring(index + 1);
                            index2 = paramValue.IndexOf("-");
                            if (index2 == -1)
                            {
                                year = int.Parse(paramValue);
                                result.StartDate = new DateTime(year, 1, 1);
                            }
                            else
                            {
                                var tmp = paramValue.Substring(0, index2);
                                year = int.Parse(tmp);
                                tmp = paramValue.Substring(index2 + 1);
                                month = int.Parse(tmp);
                                result.StartDate = new DateTime(year, month, 1);
                            }
                            break;
                        case "end":
                            paramValue = arg.Substring(index + 1);
                            index2 = paramValue.IndexOf("-");
                            if (index2 == -1)
                            {
                                year = int.Parse(paramValue);
                                result.EndDate = new DateTime(year, 1, 1);
                            }
                            else
                            {
                                var tmp = paramValue.Substring(0, index2);
                                year = int.Parse(tmp);
                                tmp = paramValue.Substring(index2 + 1);
                                month = int.Parse(tmp);
                                result.EndDate = new DateTime(year, month, 1);
                            }
                            break;
                    }
                }
            }
            if (result.UserName == null)
            {
                result.UserName = "Prodigio7";
            }
            if (result.StartDate == DateTime.MinValue)
            {
                result.StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            }
            if (result.EndDate == DateTime.MinValue)
            {
                result.EndDate = DateTime.Today;
            }
            return result;
        }

        static void WriteStats(RatingsStats stats)
        {
            WriteLine($"Start Month: {stats.StartMonth}");
            WriteLine($"End Month: {stats.EndMonth}");
            var dailyAverage = stats.TotalOfDailyRatings / (double)stats.NumberOfRatingsDays;
            WriteLine($"Average Daily Rating: {dailyAverage}");
            WriteLine($"Min Daily Rating: {stats.MinDailyRating}");
            WriteLine($"Max Daily Rating: {stats.MaxDailyRating}");
            WriteLine();
            var averageAllGames = stats.TotalOfAllRatings / (double)stats.TotalGames;
            WriteLine($"Average of All Games Rating: {averageAllGames}");
            WriteLine($"Min of All Ratings: {stats.MinOfAllRatings}");
            WriteLine($"Max of All Ratings: {stats.MaxOfAllRatings}");
            WriteLine($"Total games: {stats.TotalGames}");
            ReadKey();
        }
    }

}
