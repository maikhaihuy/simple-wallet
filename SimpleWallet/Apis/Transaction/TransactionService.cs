using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using SimpleWallet.Common;
using SimpleWallet.Configurations;
using SimpleWallet.DataAccess;
using SimpleWallet.Models;

namespace SimpleWallet.Apis.Transaction
{
    public class TransactionService : ITransactionService
    {
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        public TransactionService(IMapper mapper, IConfiguration configuration)
        {
            _mapper = mapper;
            _appSettings = configuration.Get<AppSettings>();
        }

        public async Task<List<TransactionModel>> GetTransaction(TransactionQuery getTransactionQuery)
        {
            AccountDataAccess accountDataAccess =
                new AccountDataAccess(_appSettings.ConnectionStrings.DefaultConnection);
            TransactionDataAccess transactionDataAccess =
                new TransactionDataAccess(_appSettings.ConnectionStrings.DefaultConnection);
            
            // validate
            Account accountTo = accountDataAccess.GetAccountByAccountNumber(getTransactionQuery.AccountNumber);
            if (accountTo == null)
            {
                throw new KeyNotFoundException($"{getTransactionQuery.AccountNumber} not found!");
            }

            // query data
            var transactions = transactionDataAccess.GetAllTransaction(getTransactionQuery.AccountNumber);
            
            // transfer
            List<TransactionModel> transactionModels = _mapper.Map<List<TransactionModel>>(transactions);
            return transactionModels;
        }

        public async Task<TransactionModel> DepositTransaction(DepositParam depositParam)
        {
            AccountDataAccess accountDataAccess =
                new AccountDataAccess(_appSettings.ConnectionStrings.DefaultConnection);
            TransactionDataAccess transactionDataAccess =
                new TransactionDataAccess(_appSettings.ConnectionStrings.DefaultConnection);
            
            // validate
            Account accountTo = accountDataAccess.GetAccountByAccountNumber(depositParam.AccountTo);
            if (accountTo == null)
            {
                throw new KeyNotFoundException($"{depositParam.AccountTo} not found!");
            }

            // prepare data
            DataAccess.Transaction transaction = new()
            {
                Type = TransactionType.Deposit,
                Amount = depositParam.Amount,
                AccountTo = depositParam.AccountTo,
                DateOfTransaction = DateTime.Now,
                EndBalance = accountTo.Ballance + depositParam.Amount,
            };

            // update database
            var transactionId = transactionDataAccess.AddDepositTransaction(transaction);
            
            // transfer - response
            if (transactionId < 0) return null;
            transaction = transactionDataAccess.GetTransaction(transactionId);
            TransactionModel accountModel = _mapper.Map<TransactionModel>(transaction);
            return accountModel;
        }

        public async Task<TransactionModel> WithdrawTransaction(WithdrawParam withdrawParam)
        {
            AccountDataAccess accountDataAccess =
                new AccountDataAccess(_appSettings.ConnectionStrings.DefaultConnection);
            TransactionDataAccess transactionDataAccess =
                new TransactionDataAccess(_appSettings.ConnectionStrings.DefaultConnection);
            
            // validate
            Account accountFrom = accountDataAccess.GetAccountByAccountNumber(withdrawParam.AccountFrom);
            if (accountFrom == null)
            {
                throw new KeyNotFoundException($"{withdrawParam.AccountFrom} not found!");
            }
            if (accountFrom.Ballance < withdrawParam.Amount)
            {
                throw new ValidationException($"Balance of {withdrawParam.AccountFrom} it not enough for withdraw.");
            }

            // prepare data
            DataAccess.Transaction transaction = new()
            {
                Type = TransactionType.Withdraw,
                Amount = withdrawParam.Amount,
                AccountFrom = withdrawParam.AccountFrom,
                DateOfTransaction = DateTime.Now,
                EndBalance = accountFrom.Ballance - withdrawParam.Amount,
            };
            
            // update database
            var transactionId = transactionDataAccess.AddWithDrawTransaction(transaction);
            
            // transfer - response
            if (transactionId < 0) return null;
            transaction = transactionDataAccess.GetTransaction(transactionId);
            TransactionModel accountModel = _mapper.Map<TransactionModel>(transaction);
            return accountModel;
        }

        public async Task<TransactionModel> ExchangeTransaction(ExchangeParam exchangeParam)
        {
            AccountDataAccess accountDataAccess =
                new AccountDataAccess(_appSettings.ConnectionStrings.DefaultConnection);
            TransactionDataAccess transactionDataAccess =
                new TransactionDataAccess(_appSettings.ConnectionStrings.DefaultConnection);
            
            // validate
            Account accountFrom = accountDataAccess.GetAccountByAccountNumber(exchangeParam.AccountFrom);
            Account accountTo = accountDataAccess.GetAccountByAccountNumber(exchangeParam.AccountTo);
            if (accountFrom == null)
            {
                throw new KeyNotFoundException($"{exchangeParam.AccountFrom} not found!");
            }
            if (accountTo == null)
            {
                throw new KeyNotFoundException($"{exchangeParam.AccountTo} not found!");
            }
            if (accountFrom.Ballance < exchangeParam.Amount)
            {
                throw new ValidationException($"Balance of {accountFrom.AccountNumber} it not enough for exchange.");
            }

            // prepare data
            DataAccess.Transaction transaction = new()
            {
                Type = TransactionType.Exchange,
                Amount = exchangeParam.Amount,
                AccountFrom = exchangeParam.AccountFrom,
                AccountTo = exchangeParam.AccountTo,
                DateOfTransaction = DateTime.Now,
                EndBalance = accountFrom.Ballance - exchangeParam.Amount,
            };

            // update database
            var transactionId = transactionDataAccess.AddExchangeTransaction(transaction);
            
            // transfer - response
            if (transactionId < 0) return null;
            transaction = transactionDataAccess.GetTransaction(transactionId);
            TransactionModel accountModel = _mapper.Map<TransactionModel>(transaction);
            return accountModel;
        }

        public async Task<int> Remove(List<TransactionModel> transactionModels)
        {
            TransactionDataAccess transactionDataAccess =
                new TransactionDataAccess(_appSettings.ConnectionStrings.DefaultConnection);
            List<DataAccess.Transaction> accounts = transactionModels.Select(_ => new DataAccess.Transaction {Id = _.Id}).ToList();
            return transactionDataAccess.DeleteTransactions(accounts);
        }
    }
}