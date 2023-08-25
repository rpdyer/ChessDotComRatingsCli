using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessDotComRatings.Models
{
    internal class RatingsStats
    {
        internal RatingsStats(string userName, DateTime startMonth)
        {
            UserName = userName;
            MinDailyRating = double.MaxValue;
            MaxDailyRating = double.MinValue;
            MinOfAllRatings = double.MaxValue;
            MaxOfAllRatings = double.MinValue;
            StartMonth = startMonth;
        }
        public double TotalOfAllRatings { get; set; }
        public int NumberOfAllGames { get; set; }
        public double TotalOfDailyRatings { get; set; }
        public int NumberOfRatingsDays { get; set; }
        public int TotalGames { get; set; }
        public double Average { get; set; }
        public double MinDailyRating { get; set; }
        public double MaxDailyRating { get; set; }
        public double MinOfAllRatings { get; set; }
        public double MaxOfAllRatings { get; set; }
        public DateTime LastEvaluated { get; set; }
        public int LastRatingCurrentDay { get; set; }
        public string UserName { get; set; }
        public DateTime StartMonth { get; set; }
        public DateTime EndMonth { get; set; }
        public DateTime GameEndDateLastSeen { get; set; }
        public UserGameData UserGameDataLastSeen { get; set; }
    }
}
