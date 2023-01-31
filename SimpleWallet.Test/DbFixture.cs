using System;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleWallet.Apis.Auth;
using SimpleWallet.Apis.Transaction;
using SimpleWallet.Configurations;

namespace SimpleWallet.Test
{
    public class DbFixture
    {
        public DbFixture()
        {
            var serviceCollection = new ServiceCollection();

            var config = ReadConfiguration();
            

            // Auto mapping
            serviceCollection.AddSingleton(provider => new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapping());
            }).CreateMapper());

            serviceCollection.AddSingleton<IConfiguration>(config);
            serviceCollection.AddTransient<IAuthService, AuthService>();
            serviceCollection.AddTransient<ITransactionService, TransactionService>();

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        public ServiceProvider ServiceProvider { get; private set; }
        
        private IConfiguration ReadConfiguration()
        { 
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Test.json")
                .AddEnvironmentVariables() 
                .Build();
            return config;
        }
    }
}