using CTLLunch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Interface
{
    public interface ITransaction
    {
        List<TransactionModel> GetTransactions();
        List<TransactionModel> GetTransactionByEmployee(string employee_id);
        List<TransactionModel> GetTransactionByDate(DateTime date);
        string Insert(TransactionModel transaction);

    }
}
