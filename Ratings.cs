using ChessDotComRatings.Models;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace ChessDotComRatings
{
    internal class Ratings
    {
        internal static async Task<RatingsStats> GetStats(RatingsParameters parameters)
        {
            var theMonth = parameters.StartDate;
            var stats = new RatingsStats(parameters.UserName, theMonth);
            while (theMonth <= parameters.EndDate)
            {
                var monthStr = theMonth.Month.ToString("D2");
                var fullUrl = $"https://api.chess.com/pub/player/{parameters.UserName}/games/{theMonth.Year}/{monthStr}";
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var productHeaderValue = new ProductHeaderValue("User-Agent");
                    var productInfoHeaderValue = new ProductInfoHeaderValue(productHeaderValue);
                    client.DefaultRequestHeaders.UserAgent.Add(productInfoHeaderValue);
                    var response = await client.GetAsync(fullUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        stats.EndMonth = theMonth;
                        string result = response.Content.ReadAsStringAsync().Result;
                        var games = JsonConvert.DeserializeObject<dynamic>(result);
                        Iterate(games, stats, theMonth);
                        stats.NumberOfRatingsDays++;
                        UpdateEndOfDayStats(stats, stats.UserGameDataLastSeen, stats.GameEndDateLastSeen);
                    }
                }
                theMonth = GetNextMonth(theMonth);
            }
            return stats;
        }

        private static DateTime GetNextMonth(DateTime theDate)
        {
            DateTime result;
            if (theDate.Month < 12)
            {
                result = new DateTime(theDate.Year, theDate.Month + 1, 1);
            }
            else
            {
                result = new DateTime(theDate.Year + 1, 1, 1);
            }
            return result;
        }

        public static void Iterate(dynamic games, RatingsStats stats, DateTime theMonth)
        {
            var objectType = games.GetType();
            if (objectType == typeof(Newtonsoft.Json.Linq.JObject))
            {
                Console.WriteLine("type is Object");
                foreach (var property in games)
                {
                    Console.WriteLine("property name: " + property.Name.ToString());
                    Console.WriteLine("property type: " + property.GetType().ToString());
                    Iterate(property, stats, theMonth);
                }
            }
            else if (objectType == typeof(Newtonsoft.Json.Linq.JProperty))
            {
                Console.WriteLine("type is Property");
                Iterate(games.Value, stats, theMonth);
            }
            else if (objectType == typeof(Newtonsoft.Json.Linq.JArray))
            {
                Console.WriteLine("type is Array");
                foreach (var game in games)
                {
                    AddGameToResults(game, stats, theMonth);
                }
            }
            else if (objectType == typeof(Newtonsoft.Json.Linq.JValue))
            {
                Console.WriteLine("type is Variable, value: " + games.ToString());
            }
        }

        internal static void AddGameToResults(dynamic game, RatingsStats stats, DateTime theMonth)
        {
            var objectType = game.GetType();
            string pgn = (string)game.pgn;
            string searchStr = "UTCDate ";
            string timeClass = game.time_class;
            if (timeClass != "rapid")
            {
                return;
            }
            var i = pgn.IndexOf(searchStr);
            var utcDateStr = pgn.Substring(i + searchStr.Length + 1, 10);
            var j = pgn.IndexOf("UTCTime ");
            var utcTimeStr = pgn.Substring(j + searchStr.Length + 1, 8);            
            DateTime utcGameEndDate = ExtractDateTime(utcDateStr, utcTimeStr);
            DateTime gameEndDate = utcGameEndDate.ToLocalTime();
            var userGameData = GetUserGameData(stats.UserName, game);
            UpdateAllGamesStats(stats, userGameData);

            if (stats.LastEvaluated == DateTime.MinValue || !DatesAreSameDay(stats.LastEvaluated, gameEndDate))
            {
                int daysElapsed = stats.LastEvaluated == DateTime.MinValue
                    ? 1
                    : (int)(gameEndDate - stats.LastEvaluated).TotalDays;
                if (stats.LastEvaluated != DateTime.MinValue)
                {
                    stats.NumberOfRatingsDays++;
                    UpdateEndOfDayStats(stats, userGameData, gameEndDate);
                }
                stats.LastEvaluated = gameEndDate;
            }
            stats.LastRatingCurrentDay = userGameData.RatingResult;
            stats.TotalGames++;
            stats.GameEndDateLastSeen = gameEndDate;
            stats.UserGameDataLastSeen = userGameData;
            return;
        }

        private static void UpdateAllGamesStats(RatingsStats stats, dynamic userGameData)
        {
            stats.TotalOfAllRatings += userGameData.RatingResult;
            stats.NumberOfAllGames++;
            stats.MaxOfAllRatings = Math.Max(stats.MaxOfAllRatings, userGameData.RatingResult);
            stats.MinOfAllRatings = Math.Min(stats.MinOfAllRatings, userGameData.RatingResult);
        }

        internal static void UpdateEndOfDayStats(RatingsStats stats, UserGameData userGameData, DateTime gameEndDate)
        {
            stats.MaxDailyRating = Math.Max(stats.MaxDailyRating, stats.LastRatingCurrentDay);
            stats.MinDailyRating = Math.Min(stats.MinDailyRating, stats.LastRatingCurrentDay);
            stats.TotalOfDailyRatings += stats.LastRatingCurrentDay;
        }

        internal static bool DatesAreSameDay(DateTime date1, DateTime date2)
        {
            return date1.Year == date2.Year &&
                   date1.Month == date2.Month &&
                   date1.Day == date2.Day;
        }

        internal static UserGameData GetUserGameData(string userName, dynamic game)
        {
            UserGameData gameData = new UserGameData();
            if (game.white.username == userName)
            {
                gameData.UserName = userName;
                gameData.GameResult = game.white.result;
                gameData.RatingResult = game.white.rating;
                gameData.Color = "White";
            }
            else if (game.black.username == userName)
            {
                gameData.UserName = userName;
                gameData.GameResult = game.black.result;
                gameData.RatingResult = game.black.rating;
                gameData.Color = "Black";
            }
            else
            {
                throw new ArgumentException("User Game Data not found");
            }
            return gameData;
        }

        private static DateTime ExtractDateTime(string dateStr, string timeStr)
        {
            var tmpd = dateStr.Split('.');
            var year = int.Parse(tmpd[0]);
            var month = int.Parse(tmpd[1]);
            var day = int.Parse(tmpd[2]);
            var tmph = timeStr.Split(':');
            var hour = int.Parse(tmph[0]);
            var minute = int.Parse(tmph[1]);
            var second = int.Parse(tmph[2]);
            return new DateTime(year, month, day, hour, minute, second);
        }
    }
}
