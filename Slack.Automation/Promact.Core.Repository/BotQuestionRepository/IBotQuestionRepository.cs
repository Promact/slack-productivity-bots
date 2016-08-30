using Promact.Erp.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Core.Repository.BotQuestionRepository
{
    public interface IBotQuestionRepository
    {
        void AddQuestion(Question question);
        Question FindById(int questionId);
        Question FindByQuestionType(int type);
        Question FindByTypeAndOrderNumber(int orderNumber, int type);
    }
}
