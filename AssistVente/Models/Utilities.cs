using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Web;

namespace AssistVente.Models
{
    public static class Utilities
    {
        public static string generateVentesEmailHtml()
        {
            StringBuilder builder = new StringBuilder();
            AssistVenteContext db = new AssistVenteContext();
            builder.Append("<h2 style=\"font-family: Roboto, 'Helvetica Neue', Arial, sans-serif; font-size: 1.1rem; \">Assist-Vente: Situation des ventes du "
                + DateTime.Now.Date.ToShortDateString()
                + Environment.NewLine);
            builder.Append("</h2>" + Environment.NewLine);
            builder.Append("<table border=" + 1 + " cellpadding=" + 5 + "  style=\"font-family: Roboto, 'Helvetica Neue', Arial, sans-serif; font-size: 1rem; font-weight: 300\" cellspacing=" + 0 + ">" + Environment.NewLine);
            builder.Append("<thead>" + Environment.NewLine);
            builder.Append("<tr><td>").Append("Client").Append("</td><td>").Append("Montant").Append("</td><td>").Append("Date").Append("</td></tr>" + Environment.NewLine);
            builder.Append("</thead>" + Environment.NewLine);
            builder.Append("<tbody>" + Environment.NewLine);
            var ventes = db.Ventes.ToList().Where(v => v.Date.Date == DateTime.Now.Date).ToList();
            foreach (var vente in ventes)
            {
                builder.Append("<tr><td>").Append(vente.Client.Nom).Append("</td><td>").Append(vente.Montant).Append("</td><td>").Append(vente.Date).Append("</td></tr>" + Environment.NewLine);
            }
            builder.Append("</tbody>" + Environment.NewLine);
            builder.Append("</table>" + Environment.NewLine);
            builder.Append("<br/>" + Environment.NewLine);
            builder.Append("<br/>" + Environment.NewLine);
            builder.Append("Total des ventes du " + DateTime.Now.Date.ToShortDateString() + Environment.NewLine);
            builder.Append(": " + Environment.NewLine);
            builder.Append("<strong style=\"font-family: Roboto, 'Helvetica Neue', Arial, sans-serif; font-size: 1.1rem; \">" + Environment.NewLine);
            builder.Append(ventes.Sum(v => v.Montant) + Environment.NewLine);
            builder.Append("</strong>" + Environment.NewLine);
            builder.Append("<br/>" + Environment.NewLine);
            builder.Append("<br/>" + Environment.NewLine);
            builder.Append("Génération faite le " + DateTime.Now.ToString() + Environment.NewLine);

            return builder.ToString();
        }



        public static string generateStockEmailHtml()
        {
            StringBuilder builder = new StringBuilder();
            AssistVenteContext db = new AssistVenteContext();
            builder.Append("<h2 style=\"font-family: Roboto, 'Helvetica Neue', Arial, sans-serif; font-size: 1.1rem; \">Assist-Vente: Situation du stock du "
                + DateTime.Now.Date.ToShortDateString()
                + Environment.NewLine);
            builder.Append("</h2>" + Environment.NewLine);
            builder.Append("<table border=" + 1 + " cellpadding=" + 5 + "  style=\"font-family: Roboto, 'Helvetica Neue', Arial, sans-serif; font-size: 1rem; font-weight: 300\" cellspacing=" + 0 + ">" + Environment.NewLine);
            builder.Append("<thead>" + Environment.NewLine);
            builder.Append("<tr><td>").Append("Produit").Append("</td><td>").Append("Stock").Append("</td><tr>" + Environment.NewLine);
            builder.Append("</thead>" + Environment.NewLine);
            builder.Append("<tbody>" + Environment.NewLine);
            var ventes = db.Produits.ToList();
            foreach (var produit in ventes)
            {
                builder.Append("<tr><td>").Append(produit.Nom).Append("</td><td>").Append(produit.StockDisponible).Append("</td></tr>" + Environment.NewLine);
            }
            builder.Append("</tbody>" + Environment.NewLine);
            builder.Append("</table>" + Environment.NewLine);
            builder.Append("<br/>" + Environment.NewLine);
            builder.Append("<br/>" + Environment.NewLine);
            //builder.Append("Total des ventes du " + DateTime.Now.Date.ToShortDateString() + Environment.NewLine);
            //builder.Append(": " + Environment.NewLine);
            //builder.Append("<strong style=\"font-family: Roboto, 'Helvetica Neue', Arial, sans-serif; font-size: 1.1rem; \">" + Environment.NewLine);
            //builder.Append(ventes.Sum(v => v.Montant) + Environment.NewLine);
            //builder.Append("</strong>" + Environment.NewLine);
            builder.Append("<br/>" + Environment.NewLine);
            builder.Append("<br/>" + Environment.NewLine);
            builder.Append("Génération faite le " + DateTime.Now.ToString() + Environment.NewLine);

            return builder.ToString();
        }


        public static void sendMail(string body, string subject,string sender, string recipient)
        {
            MailMessage mail = new MailMessage();
            System.Net.Mail.SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

            mail.From = new MailAddress("princegnakou@gmail.com");
            mail.To.Add("gatiengnakoU@gmail.com");
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("princegnakou@gmail.com", "Azetohprince2018");
            SmtpServer.EnableSsl = true;

            SmtpServer.Send(mail);
        }
        public static void SendNotifications()
        {
            string sender="princegnakoU@gmail.com";
            var parametre = new AssistVenteContext().Parametres.FirstOrDefault();
            if (parametre == null) { return; }
            string recipient = parametre.EmailNotifications;
            sendMail(generateStockEmailHtml(), "Assist-vente: Etat du stock du :" + DateTime.Now.ToShortDateString(),sender,recipient);
            Thread.Sleep(20000);
            sendMail(generateVentesEmailHtml(), "Assist-vente: Etat des ventes du :" + DateTime.Now.ToShortDateString(),sender,recipient);
        }
    }
}