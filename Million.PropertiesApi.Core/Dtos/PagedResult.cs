using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Million.PropertiesApi.Core.Dtos
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public long Total { get; set; }
    }
}
