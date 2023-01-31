using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleWallet.Models;

namespace SimpleWallet.Apis.Transaction
{
    public interface ITransactionService
    {
        Task<List<TransactionModel>> GetTransaction(TransactionQuery getTransactionQuery);
        Task<TransactionModel> DepositTransaction(DepositParam depositParam);
        Task<TransactionModel> WithdrawTransaction(WithdrawParam withdrawParam);
        Task<TransactionModel> ExchangeTransaction(ExchangeParam exchangeParam);
        Task<int> Remove(List<TransactionModel> transactionModels);
    }
}