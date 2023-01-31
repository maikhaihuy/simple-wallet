using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SimpleWallet.Common;
using SimpleWallet.Configurations;
using SimpleWallet.DataAccess;
using SimpleWallet.Models;

namespace SimpleWallet.Apis.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        public AuthService(IMapper mapper, IConfiguration configuration)
        {
            _mapper = mapper;
            _appSettings = configuration.Get<AppSettings>();
        }
        
        public async Task<AccountModel> Register(RegisterParam registerParam)
        {
            AccountDataAccess accountDataAccess =
                new AccountDataAccess(_appSettings.ConnectionStrings.DefaultConnection);
            // validate
            string accountNumber = Helpers.GenerateAccountNumber();
            var accountDb = accountDataAccess.GetAccountByAccountNumber(accountNumber);
            if (accountDb != null)
                throw new ValidationException($"Account number: {accountNumber} already!"); 
            
            accountDb = accountDataAccess.GetAccountByLoginName(registerParam.LoginName);
            if (accountDb != null)
                throw new ValidationException($"Login name: {registerParam.LoginName} already!");
            
            Account account = new()
            {
                LoginName = registerParam.LoginName,
                Password = Helpers.PasswordHash(registerParam.Password),
                AccountNumber = accountNumber,
                RegisterDate = DateTime.Now,
            };
            
            var accountId = accountDataAccess.AddAccount(account);
            account = accountDataAccess.GetAccount(accountId);

            AccountModel accountModel = _mapper.Map<AccountModel>(account);
            return accountModel;
        }

        public async Task<AccountModel> FindByLoginName(string loginName)
        {
            AccountDataAccess accountDataAccess =
                new AccountDataAccess(_appSettings.ConnectionStrings.DefaultConnection);

            Account account = accountDataAccess.GetAccountByLoginName(loginName);

            return _mapper.Map<AccountModel>(account);
        }

        public async Task<int> Remove(List<AccountModel> accountModels)
        {
            AccountDataAccess accountDataAccess =
                new AccountDataAccess(_appSettings.ConnectionStrings.DefaultConnection);
            List<Account> accounts = accountModels.Select(_ => new Account {Id = _.Id}).ToList();
            return accountDataAccess.DeleteAccounts(accounts);
        }
    }
}