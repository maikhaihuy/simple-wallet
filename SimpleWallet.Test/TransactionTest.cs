using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimpleWallet.Apis.Auth;
using SimpleWallet.Apis.Transaction;
using SimpleWallet.Models;
using Xunit;

namespace SimpleWallet.Test
{
    public class TransactionTest : IClassFixture<DbFixture>
    {
        private ServiceProvider _serviceProvider;

        private RegisterParam registerParam1 = new()
        {
            LoginName = "test1",
            Password = "12345678"
        };
        private RegisterParam registerParam2 = new()
        {
            LoginName = "test2",
            Password = "12345678"
        };

        public TransactionTest(DbFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async Task Deposit_Transaction_ReturnTransaction()
        {
            var authService = _serviceProvider.GetService<IAuthService>();
            var transactionService = _serviceProvider.GetService<ITransactionService>();
            // Arrange
            AccountModel accountModelFrom = await authService.Register(registerParam1);
            DepositParam depositParam = new()
            {
                AccountTo = accountModelFrom.AccountNumber,
                Amount = 10
            };
            
            // Act
            var transaction = await transactionService.DepositTransaction(depositParam);

            // Assert
            
            AccountModel accountModel = await authService.FindByLoginName(accountModelFrom.LoginName);
            Assert.True(transaction.Amount == depositParam.Amount && transaction.EndBalance == accountModel.Ballance);

            await RemoveData(new() {transaction}, new() {accountModelFrom});
        }

        [Fact]
        public async Task Withdraw_Transaction_ReturnTransaction()
        {
            var authService = _serviceProvider.GetService<IAuthService>();
            var transactionService = _serviceProvider.GetService<ITransactionService>();
            // Arrange
            AccountModel accountModelFrom = await authService.Register(registerParam1);
            DepositParam depositParam = new()
            {
                AccountTo = accountModelFrom.AccountNumber,
                Amount = 10
            };
            WithdrawParam withdrawParam = new()
            {
                AccountFrom = accountModelFrom.AccountNumber,
                Amount = 10
            };
            
            // Act
            var depositTransaction = await transactionService.DepositTransaction(depositParam);
            var withdrawTransaction = await transactionService.WithdrawTransaction(withdrawParam);
            
            // Assert
            AccountModel accountModel = await authService.FindByLoginName(accountModelFrom.LoginName);
            Assert.True(depositTransaction.Amount == depositParam.Amount &&
                        withdrawTransaction.Amount == withdrawParam.Amount &&
                        withdrawTransaction.EndBalance == accountModel.Ballance);

            await RemoveData(new() {depositTransaction, withdrawTransaction}, new() {accountModelFrom});
        }
        
        [Fact]
        public async Task Exchange_Transaction_ReturnTransaction()
        {
            var authService = _serviceProvider.GetService<IAuthService>();
            var transactionService = _serviceProvider.GetService<ITransactionService>();
            // Arrange
            AccountModel accountModelFrom = await authService.Register(registerParam1);
            System.Threading.Thread.Sleep(1000);
            AccountModel accountModelTo = await authService.Register(registerParam2);
            
            DepositParam depositParam = new()
            {
                AccountTo = accountModelFrom.AccountNumber,
                Amount = 10
            };
            ExchangeParam exchangeParam = new()
            {
                AccountFrom = accountModelFrom.AccountNumber,
                AccountTo = accountModelTo.AccountNumber,
                Amount = 6
            };
            
            // Act
            var depositTransaction = await transactionService.DepositTransaction(depositParam);
            var exchangeTransaction = await transactionService.ExchangeTransaction(exchangeParam);
            
            // Assert
            
            AccountModel accountFrom = await authService.FindByLoginName(accountModelFrom.LoginName);
            AccountModel accountTo = await authService.FindByLoginName(accountModelTo.LoginName);
            Assert.True(depositTransaction.Amount == depositParam.Amount &&
                        exchangeTransaction.Amount == exchangeParam.Amount &&
                        accountFrom.Ballance == depositParam.Amount - exchangeParam.Amount &&
                        accountTo.Ballance == exchangeParam.Amount);

            await RemoveData(new() {depositTransaction, exchangeTransaction}, new() {accountModelFrom, accountModelTo});
        }
        
        [Fact]
        public async Task Update_Account_ConcurrencyFailure()
        {
            var authService = _serviceProvider.GetService<IAuthService>();
            var transactionService = _serviceProvider.GetService<ITransactionService>();
            // Arrange
            RegisterParam registerParam = new()
            {
                LoginName = "test22",
                Password = "12345678"
            };
            var account = await authService.Register(registerParam);
            var transactions = new List<TransactionModel>();

            // Act
            var tasks = new List<Task<TransactionModel>>();
            for (int i = 0; i < 100; i++)
            {
                var task = Task.Run(async () => await transactionService.DepositTransaction(new DepositParam
                {
                    AccountTo = account.AccountNumber,
                    Amount = 100.0,
                }));
                tasks.Add(task);
            }

            // Assert
            DbUpdateConcurrencyException exp =
                await Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () => await Task.WhenAll(tasks.ToArray()));
            foreach (var task in tasks)
            {
                if (task.IsCompletedSuccessfully)
                    transactions.Add(task.Result);
            }

            await RemoveData(transactions, new() {account});
        }
        
        public async Task RemoveData(List<TransactionModel> transactionModels, List<AccountModel> accountModels)
        {
            var authService = _serviceProvider.GetService<IAuthService>();
            var transactionService = _serviceProvider.GetService<ITransactionService>();
            
            await transactionService.Remove(transactionModels);
            await authService.Remove(accountModels);
        }
    }
}