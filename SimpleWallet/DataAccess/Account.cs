using System;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SimpleWallet.Configurations;

namespace SimpleWallet.DataAccess
{
	public class Account
    {
	    public Account() { }
	    public Account(SqlDataReader reader)
	    {
		    Id = Convert.ToInt32(reader["Id"]);  
		    LoginName = reader["LoginName"].ToString();
		    Password = reader["Password"].ToString();
		    RegisterDate = Convert.ToDateTime(reader["RegisterDate"].ToString());
		    AccountNumber = reader["AccountNumber"].ToString();
		    Ballance = Convert.ToDouble(reader["Ballance"].ToString());
	    }
	    
	    public int Id { get; set; }
	    public string LoginName { get; set; }
	    public string Password { get; set; }
	    public DateTime RegisterDate { get; set; }
	    public string AccountNumber { get; set; }
	    public double Ballance { get; set; }
	    public byte[] Version { get; set; }
    }
}

