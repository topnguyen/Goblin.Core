using Elect.Core.ObjUtils;

namespace Goblin.Core.Models
{
    public abstract class GoblinApiPagedRequestModel : ElectDisposableModel
    {
        public int Skip { get; set; } = 0;

        /// <summary>
        ///     Default is 10 item 
        /// </summary>
        public int Take { get; set; } = 10;

        public string ExcludeIds { get; set; }
        
        public string IncludeIds { get; set; }

        /// <summary>
        ///     Sort by Property name
        /// </summary>
        public string SortBy { get; set; }

        /// <summary>
        ///     Sort Ascending or Descending
        /// </summary>
        /// <remarks>true is sort by Ascending, false is Descending</remarks>
        public bool IsSortAscending { get; set; }
    }
}