using Acb.Core.Dependency;
using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.Core.Helper;
using Acb.Core.Logging;
using Acb.Core.Timing;
using Spear.Sharp.Contracts;
using Spear.Sharp.Contracts.Dtos.Job;
using Spear.Sharp.Contracts.Enums;
using Quartz;
using System;
using System.Threading.Tasks;
using Spear.Sharp.Business.Domain;

namespace Spear.Sharp.Business.Scheduler
{
    public abstract class JobBase<T> : IJob where T : JobDetailDto
    {
        protected readonly ILogger Logger;


        protected JobBase()
        {
            Logger = LogManager.Logger(typeof(JobBase<>));
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
                    Logger.Error(ex.Message, ex);
                record.Status = RecordStatus.Fail;
            }
            finally
            {
                record.CompleteTime = Clock.Now;
                var repository = CurrentIocManager.Resolve<IJobContract>();
                await repository.AddRecordAsync(record);
            }
        }
    }
}
