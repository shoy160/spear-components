using Acb.Framework;
using Spear.Sharp.Contracts;
using Spear.Sharp.Contracts.Dtos.Account;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Acb.Core.Helper;
using Spear.Sharp.Business.Domain;
using Spear.Sharp.Business.Domain.Repositories;
using Spear.Sharp.Contracts.Enums;

namespace Spear.Sharp.Tests
{
    [TestClass]
    public class AccountTest : DTest
    {
        private readonly IAccountContract _contract;

        public AccountTest()
        {
            _contract = Resolve<IAccountContract>();
        }

        [TestMethod]
        public async Task MuliQueryTest()
        {
            var rep = Resolve<AccountRepository>();
            var t1 = rep.QueryAccountAsync("ichebao");
            var t2 = rep.QueryAccountAsync("icbhs");
            Print(await t1);
            Print(await t2);
        }

        [TestMethod]
        public async Task CreateProjectTest()
        {
            var result = await Resolve<IProjectContract>().AddAsync(new Contracts.Dtos.ProjectDto
            {
                Name = "test",
                Security = SecurityEnum.None,
                Code = "test",
                Desc = "test"
            });
            Print(result);
        }

        [TestMethod]
        public async Task CreateTest()
        {
            var dto = await _contract.CreateAsync(new AccountInputDto
            {
                Account = "test",
                Nick = "Test",
                Password = "123456",
                Role = AccountRole.Project,
                ProjectId = "323abe0282bfc175d05008d85a52a973"
            });
            Assert.AreNotEqual(dto, null);
            Print(dto);
        }

        [TestMethod]
        public async Task LoginTest()
        {
            var dto = await _contract.LoginAsync("ichebao", "123456");
            Assert.AreNotEqual(dto, null);
            Print(dto);
        }

        [TestMethod]
        public async Task RecordsTest()
        {
            var list = await _contract.LoginRecordsAsync(new string("b5ff8cc7-2100-ced2-d13a-08d67552b2a4"), 1, 10);
            Assert.AreNotEqual(list, null);
            Print(list);
        }
    }
}
