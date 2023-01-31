using System;
using Microsoft.Data.SqlClient;

namespace SimpleWallet.DataAccess
{
    public class Transaction
    {
        public Transaction() { }
        
        public Transaction(SqlDataReader reader)
        {
            Id = Convert.ToInt32(reader["Id"]);  
            Type = reader["Type"].ToString();
            Amount = Convert.ToDouble(reader["Amount"].ToString());  
            AccountFrom = reader["AccountFrom"].ToString();  
            AccountTo = reader["AccountTo"].ToString();
            DateOfTransaction = Convert.ToDateTime(reader["DateOfTransaction"].ToString());
            EndBalance = Convert.ToDouble(reader["EndBalance"].ToString());
        }
        public int Id { get; set; }
        public string Type { get; set; }
        public double Amount { get; set; }
        public string AccountFrom { get; set; }
        public string AccountTo { get; set; }
        public DateTime DateOfTransaction { get; set; }
        public double EndBalance { get; set; }
    }
}