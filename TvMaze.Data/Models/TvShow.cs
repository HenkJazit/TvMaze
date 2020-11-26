using System;
using System.Collections.Generic;

namespace TvMaze.Data.Models
{
    public class TvShow
    {
        public int Id { get; set; }
        public int MazeTvId { get; set; }
        public string Name { get; set; }
        public IEnumerable<CastMember> Cast { get; set; }

        /// <summary>
        /// For  compliance with https://creativecommons.org/licenses/by-sa/4.0/
        /// As specified here: http://www.tvmaze.com/api#licensing
        /// </summary>
        public string TvMazeUrl { get; set; }
    }
}
