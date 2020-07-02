namespace Goblin.Core.Models
{
    public abstract class GoblinApiPagedRequestModel : Elect.Web.Api.Models.PagedRequestModel
    {
        public string IncludeIds { get; set; }

        /// <summary>
        ///     Sort Ascending or Descending
        /// </summary>
        /// <remarks>true is sort by Ascending, false is Descending</remarks>
        public bool IsSortAscending { get; set; }
    }
}