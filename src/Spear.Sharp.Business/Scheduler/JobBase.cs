using Spear.Core.Dependency;
using Spear.Core.Exceptions;
using Spear.Core.Extensions;
using Spear.Core.Helper;
using Spear.Core.Timing;
using Spear.Sharp.Contracts;
using Spear.Sharp.Contracts.Dtos.Job;
using Spear.Sharp.Contracts.Enums;
using Quartz;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Spear.Sharp.Business.Domain;

namespace Spear.Sharp.Business.Scheduler
{
    public abstract class JobBase<T> : IJob where T : JobDetailDto
    {
        protected readonly ILogger Logger;


        protected JobBase()
        {
            Logger = CurrentIocManager.CreateLogger(GetType());
        }

        protected abstract Task ExecuteJob(T data, JobRecordDto record);

        public async Task Execute(IJobExecutionContext context)
        {
            var record = new JobRecordDto
            {
                Id = IdentityHelper.Guid32,
                StartTime = Clock.Now,
                TriggerId = context.Trigger.Key.Name
            };
            try
            {
                var data = context.JobDetail.JobDataMap.Get(Constants.JobData).CastTo<T>();
                if (data == null)
                    throw new BusiException("任务数据异常");
                record.JobId = data.Id;
                await ExecuteJob(data, record);
                record.Status = RecordStatus.Success;
            }
            catch (Exception ex)
            {
                record.Remark = ex.Message;
                if (!(ex is BusiException))
                    Logger.LogError(ex, ex.Message);
                record.Status = RecordStatus.Fail;
            }
            finally
            {
                record.CompleteTime = Clock.Now;
                var repository = CurrentIocManager.Resolve<IJobContract>();
                var result = await repository.AddRecordAsync(record);
            }
        }
    }
}
