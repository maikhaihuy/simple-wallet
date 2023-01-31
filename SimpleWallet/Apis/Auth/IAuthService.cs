using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleWallet.DataAccess;
using SimpleWallet.Models;

namespace SimpleWallet.Apis.Auth
{
    public interface IAuthService
    {
        Task<AccountModel> Register(RegisterParam registerParam);
        Task<AccountModel> FindByLoginName(string loginName);
        Task<int> Remove(List<AccountModel> accountModels);
    }
}