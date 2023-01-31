using System;
using System.ComponentModel.DataAnnotations;

namespace SimpleWallet.Apis.Transaction
{
    public class DepositParam
    {
        [Range(0, double.MaxValue, ErrorMessage = "Value should be greater than or equal to 0")]
        public double Amount { get; set; }
        [Required]
        public string AccountTo { get; set; }
    }

    public class WithdrawParam
    {
        [Range(0, double.MaxValue, ErrorMessage = "Value should be greater than or equal to 0")]
        public double Amount { get; set; }
        [Required]
        public string AccountFrom { get; set; }
    }

    public class ExchangeParam
    {
        [Range(0, double.MaxValue, ErrorMessage = "Value should be greater than or equal to 0")]
        public double Amount { get; set; }
        [Required]
        public string AccountTo { get; set; }
        [Required]
        public string AccountFrom { get; set; }
    }

    public class TransactionQuery
    {
        public string AccountNumber { get; set; }
    }
}