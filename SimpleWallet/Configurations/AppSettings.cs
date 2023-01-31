namespace SimpleWallet.Configurations
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; } = null!;
    }
    
    public class ConnectionStrings
    {
        public string DefaultConnection { get; set; } = null!;
    }
}