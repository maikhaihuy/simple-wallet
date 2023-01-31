using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SimpleWallet.Configurations;

namespace SimpleWallet.DataAccess
{
    public class TransactionDataAccess
    {
        private readonly string _connectionString;
        public TransactionDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }
  
        public IEnumerable<Transaction> GetAllTransaction(string accountNumber)  
        {  
            List<Transaction> lstTransaction = new List<Transaction>();
            using SqlConnection con = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand("spGetAllTransaction", con);  
            cmd.CommandType = CommandType.StoredProcedure;  
            cmd.Parameters.AddWithValue("@AccountNumber", accountNumber);  
            con.Open();  
            SqlDataReader rdr = cmd.ExecuteReader();  
  
            while (rdr.Read())  
            {
                lstTransaction.Add(new Transaction(rdr));  
            }  
            con.Close();
            return lstTransaction;  
        }
        
        public Transaction GetTransaction(int? id)  
        {  
            Transaction transaction = new Transaction();  
  
            using (SqlConnection con = new SqlConnection(_connectionString))  
            {  
                string sqlQuery = $"SELECT * FROM dbo.[Transaction] WHERE Id={id}";  
                SqlCommand cmd = new SqlCommand(sqlQuery, con);  
                con.Open();  
                SqlDataReader reader = cmd.ExecuteReader();  
  
                while (reader.Read())
                {
                    transaction = new Transaction(reader);
                }  
            }  
            return transaction;  
        }
        
        public int AddDepositTransaction(Transaction transaction)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            try
            {
                SqlCommand cmd = new SqlCommand("spAddDepositTransaction", con);  
                cmd.CommandType = CommandType.StoredProcedure;
                
                cmd.Parameters.AddWithValue("@Type", transaction.Type);
                cmd.Parameters.AddWithValue("@Amount", transaction.Amount);
                cmd.Parameters.AddWithValue("@AccountTo", transaction.AccountTo);  
                cmd.Parameters.AddWithValue("@EndBalance", transaction.EndBalance);
                cmd.Parameters.Add("@Id", SqlDbType.Int);
                cmd.Parameters["@Id"].Direction = ParameterDirection.Output;
            
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                
                return Convert.ToInt32(cmd.Parameters["@Id"].Value);
            }
            catch (Exception e)
            {
                if (e is SqlException {Number: 1205})
                {
                    throw new DbUpdateConcurrencyException(e.Message);
                }
                con.Close();
                throw;
            }
        }
        
        public int AddWithDrawTransaction(Transaction transaction)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand("spAddWithDrawTransaction", con);  
            cmd.CommandType = CommandType.StoredProcedure;
                
            cmd.Parameters.AddWithValue("@Type", transaction.Type);
            cmd.Parameters.AddWithValue("@Amount", transaction.Amount);
            cmd.Parameters.AddWithValue("@AccountFrom", transaction.AccountFrom);
            cmd.Parameters.AddWithValue("@EndBalance", transaction.EndBalance);
            cmd.Parameters.Add("@Id", SqlDbType.Int);
            cmd.Parameters["@Id"].Direction = ParameterDirection.Output;
            
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            
            return Convert.ToInt32(cmd.Parameters["@Id"].Value);
        }
        
        public int AddExchangeTransaction(Transaction transaction)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand("spAddExchangeTransaction", con);  
            cmd.CommandType = CommandType.StoredProcedure;
                
            cmd.Parameters.AddWithValue("@Type", transaction.Type);  
            cmd.Parameters.AddWithValue("@Amount", transaction.Amount);  
            cmd.Parameters.AddWithValue("@AccountFrom", transaction.AccountFrom);  
            cmd.Parameters.AddWithValue("@AccountTo", transaction.AccountTo);  
            cmd.Parameters.AddWithValue("@EndBalance", transaction.EndBalance);
            cmd.Parameters.Add("@Id", SqlDbType.Int);
            cmd.Parameters["@Id"].Direction = ParameterDirection.Output;
            
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            
            return Convert.ToInt32(cmd.Parameters["@Id"].Value);
        }
        
        public int DeleteTransactions(List<Transaction> transactions)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            string sqlQuery = $"DELETE FROM dbo.[Transaction] WHERE Id IN ({string.Join(',', transactions.Select(_ => _.Id).ToArray())})";
            
            SqlCommand cmd = new SqlCommand(sqlQuery, con);  
            con.Open();  
            var numberDeletedRows = cmd.ExecuteNonQuery();
            con.Close();

            return numberDeletedRows;
        }
    }
}