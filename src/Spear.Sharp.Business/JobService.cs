using Spear.AutoMapper;
using Spear.Core;
using Spear.Core.Domain;
using Spear.Core.Helper;
using Spear.Core.Timing;
using Spear.Sharp.Business.Domain.Entities;
using Spear.Sharp.Business.Domain.Repositories;
using Spear.Sharp.Contracts;
using Spear.Sharp.Contracts.Dtos.Job;
using Spear.Sharp.Contracts.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spear.Sharp.Business
{
    public class JobService : DService, IJobContract
    {
        private readonly JobRepository _repository;
        private readonly JobRecordRepository _recordRepository;
        private readonly JobTriggerRepository _triggerRepository;

        public JobService(JobRepository repository, JobRecordRepository recordRepository, JobTriggerRepository triggerRepository)
        {
            _repository = repository;
            _recordRepository = recordRepository;
            _triggerRepository = triggerRepository;
        }

        public async Task<JobDto> CreateAsync(JobInputDto inputDto)
        {
            var dto = inputDto.MapTo<JobDto>();
            dto.Id = dto.Detail.Id = IdentityHelper.Guid32;
            dto.CreateTime = Clock.Now;
            dto.Status = JobStatus.Disabled;
            var result = await _repository.InsertAsync(dto);
            return result > 0 ? dto : null;
        }

        public async Task<int> UpdateAsync(string id, JobInputDto inputDto)
        {
            var dto = inputDto.MapTo<JobDto>();
            dto.Id = id;
            var result = await _repository.UpdateAsync(dto);
            return result;
        }

        public async Task<PagedList<JobDto>> PagedListAsync(JobPagedInputDto inputDto)
        {
            var models = await _repository.QueryPagedAsync(inputDto.ProjectId, inputDto.Keyword, inputDto.Status, inputDto.Page,
                inputDto.Size);
            var jobs = models.MapPagedList<JobDto, TJob>();
            if (jobs?.List == null || !jobs.List.Any())
                return jobs;
            var ids = jobs.List.Select(t => t.Id).ToList();
            var https = await GetHttpDetailsAsync(ids);
            var times = await _repository.QueryTimesAsync(ids);
            foreach (var dto in jobs.List)
            {
                if (https.ContainsKey(dto.Id))
                    dto.Detail = https[dto.Id].MapTo<HttpDetailDto>();
                if (!times.ContainsKey(dto.Id))
                    continue;
                dto.PrevTime = times[dto.Id];
            }

            return jobs;
        }

        public Task<int> UpdateStatusAsync(string jobId, JobStatus status)
        {
            return _repository.UpdateStatusAsync(jobId, status);
        }

        public Task<JobDto> GetAsync(string jobId)
        {
            return _repository.QueryByIdAsync(jobId);
        }

        public Task<int> RemoveAsync(string jobId)
        {
            return _repository.UpdateStatusAsync(jobId, JobStatus.Delete);
        }

        public async Task<HttpDetailDto> GetHttpDetailAsync(string jobId)
        {
            var model = await _repository.QueryHttpDetailByIdAsync(jobId);
            var dto = model.MapTo<HttpDetailDto>();
            if (!string.IsNullOrWhiteSpace(model.Header))
                dto.Header = JsonConvert.DeserializeObject<IDictionary<string, string>>(model.Header);
            return dto;
        }

        public async Task<IDictionary<string, HttpDetailDto>> GetHttpDetailsAsync(List<string> jobIds)
        {
            jobIds = jobIds.Distinct().ToList();
            var models = (await _repository.QueryHttpDetailsAsync(jobIds)).ToList();
            var dtos = new Dictionary<string, HttpDetailDto>();
            foreach (var model in models)
            {
                var dto = model.MapTo<HttpDetailDto>();
                if (!string.IsNullOrWhiteSpace(model.Header))
                    dto.Header = JsonConvert.DeserializeObject<IDictionary<string, string>>(model.Header);
                dtos.Add(model.Id, dto);
            }

            return dtos;
        }

        public async Task<List<TriggerDto>> GetTriggersAsync(string jobId)
        {
            var dto = await _triggerRepository.QueryByJobIdAsync(jobId);
            return (dto ?? new List<TriggerDto>()).ToList();
        }

        public async Task<TriggerDto> GetTriggerAsync(string triggerId)
        {
            var model = await _triggerRepository.QueryByIdAsync(triggerId);
            return model.MapTo<TriggerDto>();
        }

        public Task<int> CreateTriggerAsync(string jobId, TriggerInputDto inputDto)
        {
            var model = inputDto.MapTo<TJobTrigger>();
            model.Id = IdentityHelper.Guid32;
            model.Status = (byte)TriggerStatus.Disable;
            model.JobId = jobId;
            model.CreateTime = Clock.Now;
            return _triggerRepository.InsertAsync(model);
        }

        public Task<int> UpdateTriggerAsync(string triggerId, TriggerInputDto inputDto)
        {
            var model = inputDto.MapTo<TJobTrigger>();
            model.Id = triggerId;
            return _triggerRepository.UpdateAsync(model);
        }

        public Task<int> UpdateTriggerStatusAsync(string triggerId, TriggerStatus status)
        {
            return _triggerRepository.UpdateStatusAsync(triggerId, status);
        }

        public Task<IDictionary<string, List<TriggerDto>>> GetTriggersAsync(List<string> jobIds)
        {
            return _triggerRepository.QueryByJobIdsAsync(jobIds);
        }

        public async Task<int> AddRecordAsync(JobRecordDto dto)
        {
            var model = dto.MapTo<TJobRecord>();
            return await _recordRepository.InsertAsync(model);
        }

        public async Task<PagedList<JobRecordDto>> RecordsAsync(string jobId, string triggerId = null, int page = 1, int size = 10)
        {
            return await _recordRepository.QueryPagedByJobIdAsync(jobId, triggerId, page, size);
        }
    }
}
