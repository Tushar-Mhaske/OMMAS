using Mvc.Mailer;
using PMGSY.Models.Common;
using System.Collections.Generic;
using System.Net.Mail;

namespace PMGSY.Mailers
{ 
    public interface IUserMailer
    {
        MailMessage SendMailCustomFunc(SendMailCustomFuncModel eMailModel, MailMessage mailMessage, string viewName, Dictionary<string, string> linkedResources);
		//MvcMailMessage PasswordReset();
	}
}