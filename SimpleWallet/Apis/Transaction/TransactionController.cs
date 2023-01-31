using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SimpleWallet.Models;

namespace SimpleWallet.Apis.Transaction
{
    [ApiController]
    [Route("api/v{version:ApiVersion}/transaction")]
    [ApiVersion("1.0")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<ActionResult> Transactions(string accountNumber)
        {
            List<TransactionModel> transactionModels =
                await _transactionService.GetTransaction(new() {AccountNumber = accountNumber});
            return Ok(transactionModels);
        }
        
        [HttpPost("deposit")]
        public async Task<ActionResult> Deposit([FromBody] DepositParam depositParam)
        {
            try
            {
                TransactionModel transactionModel = await _transactionService.DepositTransaction(depositParam);;
                return Ok(transactionModel);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            
        }
        
        [HttpPost("withdraw")]
        public async Task<ActionResult> Withdraw([FromBody] WithdrawParam withdrawParam)
        {
            TransactionModel transactionModel = await _transactionService.WithdrawTransaction(withdrawParam);
            return Ok(transactionModel);
        }
        
        [HttpPost("exchange")]
        public async Task<ActionResult> Exchange([FromBody] ExchangeParam exchangeParam)
        {
            TransactionModel transactionModel = await _transactionService.ExchangeTransaction(exchangeParam);;
            return Ok(transactionModel);
        }
    }
}