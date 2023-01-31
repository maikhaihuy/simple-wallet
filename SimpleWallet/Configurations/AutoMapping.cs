using System.Data;
using AutoMapper;
using SimpleWallet.Models;

namespace SimpleWallet.Configurations
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<AccountModel, DataAccess.Account>().ReverseMap();
            CreateMap<TransactionModel, DataAccess.Transaction>().ReverseMap();
        }
    }
}