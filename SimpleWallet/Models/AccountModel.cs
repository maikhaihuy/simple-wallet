using System;

namespace SimpleWallet.Models
{
    public class AccountModel
    {
        public int Id { get; set; }
        public string LoginName { get; set; }
        public DateTime RegisterDate { get; set; }
        public string AccountNumber { get; set; }
        public double Ballance { get; set; }
    }
}