﻿using Spear.Core.Exceptions;
using Spear.Core.Helper;
using Quartz;
using Quartz.Impl;
using Spear.Sharp.Business.Domain;
using Spear.Sharp.Contracts;
using Spear.Sharp.Contracts.Dtos.Job;
using Spear.Sharp.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Spear.Core.Dependency;

namespace Spear.Sharp.Business.Scheduler
{
    public class SchedulerCenter : ISchedulerContract
    {
        private IScheduler _scheduler;
        private readonly IJobContract _jobContract;
        private readonly ILogger _logger;

        public SchedulerCenter(IJobContract jobContract)
        {
            _logger = CurrentIocManager.CreateLogger<SchedulerCenter>();
            _jobContract = jobContract;
        }

        #region Private
        private async Task StartScheduler()
        {
            var props = new NameValueCollection
            {
                {"quartz.serializer.type", "binary"}
            };
            var factory = new StdSchedulerFactory(props);
            _scheduler = await factory.GetScheduler();
            var jobs = await _jobContract.PagedListAsync(new JobPagedInputDto { Status = JobStatus.Enabled, Size = 1000 });
            foreach (var dto in jobs.List)
            {
                if (dto.Status != JobStatus.Enabled)
                    continue;
                await RunJob(dto);
            }

            await _scheduler.Start();
        }

        private async Task RunJob(JobDto dto, ITrigger trigger = null)
        {
            switch (dto.Type)
            {
                case JobType.Http:
                    await StartHttpJob(dto, trigger);
                    break;
            }
        }

        private async Task StartHttpJob(JobDto dto, ITrigger trigger)
        {
            var builder = JobBuilder.Create<JobHttp>();
            var map = new JobDataMap { { Constants.JobData, dto.Detail } };
            var jobDetail = builder
                .WithIdentity(dto.Id)
                .SetJobData(map)
                .Build();
            if (trigger != null)
            {
                await _scheduler.ScheduleJob(jobDetail, trigger);
            }
            else
            {
                var triggers = new List<ITrigger>();
                var list = await _jobContract.GetTriggersAsync(dto.Id);
                foreach (var triggerDto in list.Where(t => t.Status == TriggerStatus.Enable))
                {
                    var triggerBuilder = GetTrigger(triggerDto);
                    if (triggerBuilder == null)
                        continue;
                    triggerBuilder.ForJob(jobDetail);

                    triggers.Add(triggerBuilder.Build());
                }

                if (triggers.Any())
                    await _scheduler.ScheduleJob(jobDetail, triggers.AsReadOnly(), true);
            }
        }

        private static TriggerBuilder GetTrigger(TriggerDto trigger)
        {
            var triggerBuilder = TriggerBuilder.Create()
                .WithIdentity(trigger.Id);
            if (trigger.Start.HasValue)
                triggerBuilder.StartAt(trigger.Start.Value);
            if (trigger.Expired.HasValue)
                triggerBuilder.EndAt(trigger.Expired.Value);
            switch (trigger.Type)
            {
                case TriggerType.Cron:
                    //当设置了start或者end时，启动调度时会自动执行，添加WithMisfireHandlingInstructionDoNothing阻止
                    triggerBuilder.WithCronSchedule(trigger.Cron, b => b.WithMisfireHandlingInstructionDoNothing());
                    break;
                case TriggerType.Simple:
                    if (trigger.Times == 0)
                        return null;
                    triggerBuilder
                        .WithSimpleSchedule(b =>
                        {
                            var sb = b.WithIntervalInSeconds(trigger.Interval);
                            if (trigger.Times > 1)
                                sb.WithRepeatCount(trigger.Times - 1);
                            else if (trigger.Times == -1)
                                sb.RepeatForever();
                        });
                    break;
            }
            return triggerBuilder;
        }
        #endregion

        /// <summary> 添加任务 </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task AddJob(JobDto dto)
        {
            if (dto.Status == JobStatus.Enabled)
                await RunJob(dto);
        }

        /// <summary> 是否在运行 </summary>
        public bool IsRunning => _scheduler.IsStarted && !_scheduler.InStandbyMode;

        /// <summary> 开启调度器 </summary>
        /// <returns></returns>
        public async Task Start()
        {
            if (_scheduler == null)
            {
                await StartScheduler();
                return;
            }
            //开启调度器
            if (_scheduler.InStandbyMode)
            {
                await _scheduler.Start();
                _logger.LogInformation("任务调度启动！");
            }
        }

        /// <summary> 停止任务调度 </summary>
        public async Task Stop()
        {
            if (_scheduler == null)
                return;
            //判断调度是否已经关闭
            if (!_scheduler.InStandbyMode)
            {
                //等待任务运行完成
                await _scheduler.Standby(); //TODO  注意：Shutdown后Start会报错，所以这里使用暂停。
                _logger.LogInformation("任务调度暂停！");
            }
        }

        /// <summary> 立即执行 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public async Task TriggerJob(string jobId)
        {
            //立即执行
            var dto = await _jobContract.GetAsync(jobId);
            if (dto == null)
                throw new BusiException("任务不存在");
            var trigger = TriggerBuilder.Create()
                .WithIdentity($"{IdentityHelper.Guid32}")
                .StartNow()
                .Build();
            dto.Id = IdentityHelper.Guid32;
            await RunJob(dto, trigger);
        }

        /// <summary> 暂停触发器 </summary>
        /// <param name="triggerId"></param>
        /// <returns></returns>
        public async Task PauseTrigger(string triggerId)
        {
            await _scheduler.PauseTrigger(new TriggerKey(triggerId));
        }

        /// <summary> 重置触发器 </summary>
        /// <param name="triggerId"></param>
        /// <returns></returns>
        public async Task ResetTrigger(string triggerId)
        {
            var key = new TriggerKey(triggerId);
            if (!await _scheduler.CheckExists(key))
                return;
            var dto = await _jobContract.GetTriggerAsync(triggerId);
            if (dto == null || dto.Status != TriggerStatus.Enable)
                return;
            var job = await _scheduler.GetJobDetail(new JobKey(dto.JobId));
            if (job == null)
                return;
            var triggerBuilder = GetTrigger(dto);
            if (triggerBuilder == null)
                return;
            var trigger = triggerBuilder.ForJob(job).Build();
            await _scheduler.RescheduleJob(key, trigger);
        }

        /// <summary> 恢复触发器 </summary>
        /// <param name="triggerId"></param>
        /// <returns></returns>
        public async Task ResumeTrigger(string triggerId)
        {
            var triggerKey = new TriggerKey(triggerId);
            if (await _scheduler.CheckExists(triggerKey))
            {
                await _scheduler.ResumeTrigger(triggerKey);
            }
            else
            {
                var dto = await _jobContract.GetTriggerAsync(triggerId);
                if (dto == null || dto.Status != TriggerStatus.Enable)
                    return;
                var job = await _scheduler.GetJobDetail(new JobKey(dto.JobId));
                if (job == null)
                {
                    //任务不存在则重新run
                    var jobDto = await _jobContract.GetAsync(dto.JobId);
                    if (jobDto.Status == JobStatus.Enabled)
                    {
                        await RunJob(jobDto);
                    }
                    return;
                }

                var triggerBuilder = GetTrigger(dto);
                if (triggerBuilder == null)
                    return;
                var trigger = triggerBuilder.ForJob(job).Build();
                await _scheduler.ScheduleJob(trigger);
            }
        }

        /// <summary> 暂停 指定的计划 </summary>
        /// <param name="jobId">任务Id</param>
        /// <returns></returns>
        public async Task PauseJob(string jobId)
        {
            try
            {
                var key = new JobKey(jobId);
                if (!await _scheduler.CheckExists(key))
                    return;
                await _scheduler.PauseJob(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"暂停任务失败！{ex.Message}");
            }
        }

        /// <summary> 删除 指定的计划 </summary>
        /// <param name="jobId">任务Id</param>
        /// <returns></returns>
        public async Task RemoveJob(string jobId)
        {
            try
            {
                var key = new JobKey(jobId);
                await _scheduler.DeleteJob(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"删除任务失败！{ex.Message}");
            }
        }

        /// <summary> 恢复运行暂停的任务 </summary>
        /// <param name="jobId">任务Id</param>
        public async Task ResumeJob(string jobId)
        {
            try
            {
                var key = new JobKey(jobId);
                if (!await _scheduler.CheckExists(key))
                {
                    var dto = await _jobContract.GetAsync(jobId);
                    if (dto.Status == JobStatus.Enabled)
                        await RunJob(dto);
                    return;
                }
                //任务已经存在则暂停任务
                await _scheduler.ResumeJob(key);
                _logger.LogInformation($"任务[{jobId}]恢复运行");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"恢复任务失败！{ex.Message}");
            }
        }

        /// <summary> 获取触发器执行时间 </summary>
        /// <returns></returns>
        public async Task FillJobsTime(IEnumerable<JobDto> dtos)
        {
            foreach (var dto in dtos)
            {
                var key = new JobKey(dto.Id);
                var triggers = await _scheduler.GetTriggersOfJob(key);
                if (triggers == null || !triggers.Any())
                    continue;
                foreach (var trigger in triggers)
                {
                    var status = await _scheduler.GetTriggerState(trigger.Key);
                    if (status != TriggerState.Normal)
                        continue;
                    var prev = trigger.GetPreviousFireTimeUtc()?.LocalDateTime;
                    var next = trigger.GetNextFireTimeUtc()?.LocalDateTime;
                    if (prev.HasValue && (!dto.PrevTime.HasValue || prev > dto.PrevTime))
                        dto.PrevTime = prev;
                    if (next.HasValue && (!dto.NextTime.HasValue || next < dto.NextTime))
                        dto.NextTime = next;
                }
            }
        }

        /// <summary> 获取触发器执行时间 </summary>
        /// <returns></returns>
        public async Task FillTriggersTime(IEnumerable<TriggerDto> dtos)
        {
            foreach (var dto in dtos)
            {
                var key = new TriggerKey(dto.Id);
                var status = await _scheduler.GetTriggerState(key);
                if (status != TriggerState.Normal)
                    continue;
                var trigger = await _scheduler.GetTrigger(key);
                if (trigger == null)
                    continue;
                var prevTime = trigger.GetPreviousFireTimeUtc()?.LocalDateTime;
                if (prevTime.HasValue)
                    dto.PrevTime = prevTime;
                dto.NextTime = trigger.GetNextFireTimeUtc()?.LocalDateTime;
            }
        }
    }
}
