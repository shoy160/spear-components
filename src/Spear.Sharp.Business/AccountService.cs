﻿using Spear.AutoMapper;
using Spear.Core;
using Spear.Core.Domain;
using Spear.Core.Exceptions;
using Spear.Core.Extensions;
using Spear.Core.Helper;
using Spear.Core.Timing;
using Spear.Sharp.Business.Domain.Entities;
using Spear.Sharp.Business.Domain.Repositories;
using Spear.Sharp.Contracts;
using Spear.Sharp.Contracts.Dtos.Account;
using Spear.Sharp.Contracts.Enums;
using System;
using System.Threading.Tasks;

namespace Spear.Sharp.Business
{
    public class AccountService : DService, IAccountContract
    {
        private readonly AccountRepository _repository;
        private readonly AccounrRecordRepository _recordRepository;

        public AccountService(AccountRepository repository, AccounrRecordRepository recordRepository)
        {
            _repository = repository;
            _recordRepository = recordRepository;
        }

        public async Task<AccountDto> CreateAsync(AccountInputDto inputDto)
        {
            if (await _repository.ExistsAccountAsync(inputDto.Account))
                throw new BusiException("登录帐号已存在");
            var model = inputDto.MapTo<TAccount>();
            model.Id = IdentityHelper.Guid32;
            model.PasswordSalt = IdentityHelper.Guid16;
            model.Password = $"{model.Password},{model.PasswordSalt}".Md5();
            model.CreateTime = Clock.Now;
            var result = await _repository.InsertAsync(model);
            if (result <= 0)
                throw new BusiException("创建账户失败");
            return model.MapTo<AccountDto>();
        }

        public async Task<AccountDto> LoginAsync(string account, string password)
        {
            var model = await _repository.QueryAccountAsync(account);
            if (model == null)
                throw new BusiException("登录帐号不存在");
            var record = new TAccountRecord
            {
                Id = IdentityHelper.Guid32,
                AccountId = model.Id,
                CreateTime = Clock.Now,
                //CreateIp = AcbHttpContext.ClientIp,
                //UserAgent = AcbHttpContext.UserAgent ?? "client"
            };
            if (!string.Equals($"{password},{model.PasswordSalt}".Md5(), model.Password))
            {
                record.Status = (byte)RecordStatus.Fail;
                record.Remark = "登录密码不正确";
            }
            else
            {
                record.Status = (byte)RecordStatus.Success;
                record.Remark = "登录成功";
            }

            await _recordRepository.InsertAsync(record);
            if (record.Status != (short)RecordStatus.Success)
                throw new BusiException(record.Remark);
            return model.MapTo<AccountDto>();
        }

        public Task<int> UpdateAsync(string id, AccountInputDto inputDto)
        {
            var model = inputDto.MapTo<TAccount>();
            model.Id = id;
            return _repository.UpdateAsync(model);
        }

        public async Task<PagedList<AccountRecordDto>> LoginRecordsAsync(string id, int page = 1, int size = 10)
        {
            var paged = await _recordRepository.QueryPagedListAsync(id, page, size);
            return paged.MapPagedList<AccountRecordDto, TAccountRecord>();
        }
    }
}
