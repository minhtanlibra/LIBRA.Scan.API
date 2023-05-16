using AutoMapper;
using LIBRA.Scan.Data.Repositories.Constracts;
using LIBRA.Scan.Entities.Dtos;
using LIBRA.Scan.Entities.Entities;
using LIBRA.Scan.Service.Constracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Service
{
    public class BatchService : IBatchService
    {
        public IBatchRepo batchRepo;
        public IMapper mapper;

        public BatchService(IBatchRepo batchRepo, IMapper mapper)
        {
            this.batchRepo = batchRepo;
            this.mapper = mapper;   
        }

        public async Task<IEnumerable<BatchDto>> Get(Expression<Func<Batch, bool>> predicate)
        {
            var batch = await batchRepo.ListAsync(predicate, q => (IOrderedQueryable<Batch>)q.Include(x => x.Job));
            return mapper.Map<IEnumerable<BatchDto>>(batch);
        }

        public async Task<BatchDto> GetSingle(Expression<Func<Batch, bool>> predicate)
        {
            var batch = await batchRepo.Find(predicate, q => q.Include(x=>x.Job).Include(y=>y.Status));
            var a = batch.Job;
            var b = batch.Status;
            return mapper.Map<BatchDto>(batch);
        }
        public async Task<bool> Add(BatchDto batchDto)
        {
            Batch batch = new Batch()
            {
                JobId = batchDto.JobId,
                CreatedDate = batchDto.CreatedDate,
                Name = batchDto.Name,
                Deleted = false,
                StatusId = batchDto.StatusId
            };
            try
            {
                await batchRepo.AddAsync(batch, false);
                return true;
            }
            catch {}
            return false;
        }
    }
}
