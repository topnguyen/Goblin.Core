using System.Collections.Generic;
using Elect.Core.ObjUtils;

namespace Goblin.Core.Models
{
    public class GoblinApiPagedResponseModel<T> : ElectDisposableModel where T : class, new()
    {
        public long Total { get; set; }

        public IEnumerable<T> Items { get; set; }

        public Dictionary<string, object> AdditionalData { get; set; } = new Dictionary<string, object>();
    }
}