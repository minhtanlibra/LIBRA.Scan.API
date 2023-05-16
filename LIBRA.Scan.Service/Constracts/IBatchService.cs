using LIBRA.Scan.Entities.Dtos;
using LIBRA.Scan.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Service.Constracts
{
    public interface IBatchService
    {
        Task<BatchDto> GetSingle(Expression<Func<Batch, bool>> predicate);
        Task<IEnumerable<BatchDto>> Get(Expression<Func<Batch, bool>> predicate);
        Task<bool> Add(BatchDto entity);
    }
}
