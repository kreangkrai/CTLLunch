using CTLLunch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Interface
{
    public interface ITransaction
    {
        Task<List<TransactionModel>> GetTransactions();
        Task<List<TransactionModel>> GetTransactionByEmployee(string employee_id);
        Task<List<TransactionModel>> GetTransactionByDate(DateTime date);
        Task<List<TransactionModel>> GetTransactionByMonth(string month);
        Task<string> Insert(TransactionModel transaction);

    }
}
