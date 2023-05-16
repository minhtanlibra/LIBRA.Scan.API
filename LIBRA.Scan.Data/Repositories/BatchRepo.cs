using LIBRA.Scan.Data.EFs;
using LIBRA.Scan.Data.Repositories.Constracts;
using LIBRA.Scan.Entities;
using LIBRA.Scan.Entities.Dtos;
using LIBRA.Scan.Entities.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Data.Repositories
{
    public class BatchRepo : GenericRepository<Batch>, IBatchRepo
    {
        public BatchRepo(ScandbContext context) : base(context)
        {
        }
    }
}
