using Mvc.Mailer;
using PMGSY.Models.Common;
using System.Collections.Generic;
using System.Net.Mail;
using System.Web.Mvc;

namespace PMGSY.Mailers
{ 
    public class UserMailer : MailerBase, IUserMailer 	
	{
		public UserMailer()
		{
			MasterName="_Layout";
		}

        //public virtual MvcMailMessage Welcome(string subject, string mailTo, string attachmentPath)
        public virtual MailMessage SendMailCustomFunc(SendMailCustomFuncModel eMailModel, MailMessage mailMessage, string viewName, Dictionary<string, string> linkedResources)
		{
            ViewData = new ViewDataDictionary(eMailModel);
            PopulateBody(mailMessage, viewName, linkedResources);
            return mailMessage;
		}
  	}
}