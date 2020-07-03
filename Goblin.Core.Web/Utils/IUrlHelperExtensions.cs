using Elect.Web.Models;
using Goblin.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Goblin.Core.Web.Utils
{
    public static class IUrlHelperExtensions
    {
        public static GoblinApiPagedMetaResponseModel<TRequest, TResponse> GetGoblinApiPagedMetaResponseModel<TRequest, TResponse>(
            this IUrlHelper urlHelper,
            TRequest pagedRequestModel,
            GoblinApiPagedResponseModel<TResponse> pagedResponseModel,
            HttpMethod method = HttpMethod.GET)
            where TRequest : GoblinApiPagedRequestModel
            where TResponse : class, new()
        {
            GoblinApiPagedMetaResponseModel<TRequest, TResponse> pagedMetaModel = new GoblinApiPagedMetaResponseModel<TRequest, TResponse>(urlHelper, pagedRequestModel, pagedResponseModel, method);

            return pagedMetaModel;
        }
    }
}