using System;

namespace SimpleWallet.Models
{
    public class TransactionModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public double Amount { get; set; }
        public string AccountFrom { get; set; }
        public string AccountTo { get; set; }
        public DateTime DateOfTransaction { get; set; }
        public double EndBalance { get; set; }
    }
}