using Elect.Core.ObjUtils;

namespace Goblin.Core.Models
{
    public abstract class GoblinApiSubmitRequestModel : ElectDisposableModel
    {
        /// <summary>
        ///     UserId Key
        /// </summary>
        public long? LoggedInUserId { get; set; }
    }
}