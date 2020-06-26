namespace Goblin.Core.Models
{
    public class GoblinApiRequestModel
    {
        /// <summary>
        ///     Authorization Key
        /// </summary>
        public string AuthorizationKey { get; set; }
        
        /// <summary>
        ///     UserId Key
        /// </summary>
        public long LoggedInUserId { get; set; }
    }
}