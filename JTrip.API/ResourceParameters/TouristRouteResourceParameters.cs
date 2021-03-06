using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JTrip.API.ResourceParameters
{
    public class TouristRouteResourceParameters
    {
        public string OrderBy { get; set; }
        public string Keyword { get; set; }
        public string RatingOperator { get; set; }
        public int? RatingValue { get; set; }
        private string _rating;

        public string Rating
        {
            get => _rating;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    var regex = new Regex(@"([A-Za-z0-9\-]+)(\d+)");
                    var match = regex.Match(value);
                    if (match.Success)
                    {
                        RatingOperator = match.Groups[1].Value;
                        RatingValue = Int32.Parse(match.Groups[2].Value);
                    }
                }

                _rating = value;
            }
        }

        public string Fields { get; set; }
    }
}
