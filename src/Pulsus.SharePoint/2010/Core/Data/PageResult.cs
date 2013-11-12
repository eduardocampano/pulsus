using System.Collections.Generic;

namespace Pulsus.SharePoint.Core.Data
{
    internal class PageResult<T> where T : class
    {
        public PageResult(IEnumerable<T> data, int total)
        {
            Data = data;
            Total = total;
        }   

        public long Total { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
}
