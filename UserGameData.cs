using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessDotComRatings.Models
{
    internal class UserGameData
    {
        public string Color { get; set; }
        public string GameResult { get; set; }
        public int RatingResult { get; set; }
        public string UserName { get; set; }
    }
}
