using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimpleWallet.Apis.Auth;
using SimpleWallet.Models;
using Xunit;

namespace SimpleWallet.Test
{
    public class AuthServiceTest : IClassFixture<DbFixture>
    {
        private ServiceProvider _serviceProvider;

        public AuthServiceTest(DbFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }
        
        [Fact]
        public async Task Register_Account_ReturnAccount()
        {
            var authService = _serviceProvider.GetService<IAuthService>();
            // Arrange
            RegisterParam registerParam = new()
            {
                LoginName = "admin",
                Password = "12345678"
            };

            // Act
            var result = await authService.Register(registerParam);
            
            // Assert
            Assert.True((result.LoginName == registerParam.LoginName && result.Ballance == 0));

            await RemoveData(new() {result});
        }
        
        [Fact]
        public async Task Register_Account_DuplicateUsername()
        {
            var authService = _serviceProvider.GetService<IAuthService>();
            // Arrange
            RegisterParam registerParam1 = new()
            {
                LoginName = "test1",
                Password = "12345678"
            };
            
            RegisterParam registerParam2 = new()
            {
                LoginName = "test1",
                Password = "12345678"
            };

            // Act
            var result1 = await authService.Register(registerParam1);
            ValidationException exp = await Assert.ThrowsAsync<ValidationException>(() => authService.Register(registerParam2));

            // Assert
            Assert.Equal($"Login name: {registerParam2.LoginName} already!", exp.Message);

            await RemoveData(new() {result1});
        }
        
        public async Task RemoveData(List<AccountModel> accountModels)
        {
            var authService = _serviceProvider.GetService<IAuthService>(); 
            await authService.Remove(accountModels);
        }
    }
}