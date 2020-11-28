using Spear.AutoMapper;
using Spear.Core;
using Spear.Core.Domain;
using Spear.Core.Extensions;
using Spear.Core.Helper;
using Spear.Core.Timing;
using Spear.Sharp.Business.Domain.Entities;
using Spear.Sharp.Business.Domain.Repositories;
using Spear.Sharp.Contracts;
using Spear.Sharp.Contracts.Dtos;
using System;
using System.Threading.Tasks;

namespace Spear.Sharp.Business
{
    public class ProjectService : DService, IProjectContract
    {
        private readonly ProjectRepository _repository;

        public ProjectService(ProjectRepository repository)
        {
            _repository = repository;
        }

        public Task<int> AddAsync(ProjectDto dto)
        {
            TProject model = dto.MapTo<TProject>();
            model.Id = IdentityHelper.Guid32;
            model.CreateTime = Clock.Now;
            if (model.Secret.IsNullOrEmpty())
                model.Secret = IdentityHelper.Guid16;
            return _repository.InsertAsync(model);
        }

        public Task<int> UpdateAsync(ProjectDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<ProjectDto> DetailByCodeAsync(string code)
        {
            return _repository.QueryByCodeAsync(code);
        }

        public async Task<ProjectDto> DetailAsync(string id)
        {
            var model = await _repository.QueryByIdAsync(id);
            return model.MapTo<ProjectDto>();
        }

        public Task<PagedList<ProjectDto>> PagedListAsync(int page = 1, int size = 10)
        {
            throw new NotImplementedException();
        }
    }
}
