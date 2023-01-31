using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SimpleWallet.Configurations;

namespace SimpleWallet.DataAccess
{
    public class AccountDataAccess
    {
        private readonly string _connectionString;

        public AccountDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }
        
        public Account GetAccount(int? id)  
        {  
            Account account = new Account();

            using SqlConnection con = new SqlConnection(_connectionString);
            string sqlQuery = $"SELECT * FROM Account WHERE Id={id}";  
            SqlCommand cmd = new SqlCommand(sqlQuery, con);  
            con.Open();  
            SqlDataReader reader = cmd.ExecuteReader();
            if (!reader.HasRows) return null;
  
            while (reader.Read())
            {
                account = new Account(reader);
            }

            return account;  
        }

        public Account GetAccountByLoginName(string loginName)
        {
            Account account = new Account();

            using SqlConnection con = new SqlConnection(_connectionString);
            string sqlQuery = $"SELECT * FROM Account WHERE LoginName=\'{loginName}\'";  
            SqlCommand cmd = new SqlCommand(sqlQuery, con);  
            con.Open();  
            SqlDataReader reader = cmd.ExecuteReader();
            if (!reader.HasRows) return null;
  
            while (reader.Read())
            {
                account = new Account(reader);
            }

            return account;  
        }
        
        public Account GetAccountByAccountNumber(string accountNumber)
        {
            Account account = new Account();

            using SqlConnection con = new SqlConnection(_connectionString);
            string sqlQuery = $"SELECT * FROM Account WHERE AccountNumber=\'{accountNumber}\'";  
            SqlCommand cmd = new SqlCommand(sqlQuery, con);  
            con.Open();  
            SqlDataReader reader = cmd.ExecuteReader();
            if (!reader.HasRows) return null;
  
            while (reader.Read())
            {
                account = new Account(reader);
            }

            return account;  
        }
        
        public int AddAccount(Account account)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand("spAddAccount", con);  
            cmd.CommandType = CommandType.StoredProcedure;
            
            cmd.Parameters.AddWithValue("@LoginName", account.LoginName);  
            cmd.Parameters.AddWithValue("@Password", account.Password);  
            cmd.Parameters.AddWithValue("@AccountNumber", account.AccountNumber);
            cmd.Parameters.Add("@Id", SqlDbType.Int);
            cmd.Parameters["@Id"].Direction = ParameterDirection.Output;
            
            con.Open();  
            cmd.ExecuteNonQuery();  
            con.Close();
            return Convert.ToInt32(cmd.Parameters["@Id"].Value);
        }
        
        public void UpdateAccount(Account account)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand("spAddAccount", con);  
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", account.Id);
            cmd.Parameters.AddWithValue("@Ballance", account.Ballance);
            con.Open();  
            cmd.ExecuteNonQuery();  
            con.Close();
        }

        public int DeleteAccounts(List<Account> accounts)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            string sqlQuery = $"DELETE FROM Account WHERE Id IN ({string.Join(',', accounts.Select(_ => _.Id).ToArray())})";
            
            SqlCommand cmd = new SqlCommand(sqlQuery, con);  
            con.Open();  
            var numberDeletedRows = cmd.ExecuteNonQuery();
            con.Close();

            return numberDeletedRows;
        }
    }
}