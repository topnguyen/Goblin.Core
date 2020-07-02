namespace Goblin.Core.Models
{
    public abstract class GoblinApiSubmitRequestModel
    {
        /// <summary>
        ///     UserId Key
        /// </summary>
        public long? LoggedInUserId { get; set; }
    }
}