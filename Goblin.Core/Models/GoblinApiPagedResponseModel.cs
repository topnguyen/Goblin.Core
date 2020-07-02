namespace Goblin.Core.Models
{
    public abstract class GoblinApiPagedResponseModel<T>: Elect.Web.Api.Models.PagedResponseModel<T> where T : class, new()
    {
    }
}