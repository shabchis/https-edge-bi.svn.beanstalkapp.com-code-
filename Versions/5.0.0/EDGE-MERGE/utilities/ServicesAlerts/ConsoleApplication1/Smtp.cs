﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Net.Mail;
using System.Net;

namespace ServicesAlert
{
    class Smtp
    {
       internal static SmtpClient GetSmtpConnection(out string to , out string from)
        {
            try
            {
                IDictionary smtpCon = Config.GetSection("SmtpConnection");
                SmtpClient smtp = new SmtpClient(smtpCon["server"].ToString(), Int32.Parse((smtpCon["port"].ToString())));
                smtp.Credentials = new NetworkCredential(smtpCon["user"].ToString(), smtpCon["pass"].ToString());
                smtp.UseDefaultCredentials = Boolean.Parse(smtpCon["UseDefaultCredentials"].ToString());
                smtp.EnableSsl = Boolean.Parse(smtpCon["EnableSsl"].ToString());

                to = smtpCon["to"].ToString();
                from = smtpCon["from"].ToString();
                return smtp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
