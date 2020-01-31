using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AssistVente.Models
{
    public class Parametre
    {
        [EmailAddress]
        [Display(Name ="Email destinataire des rapports")]
        public string EmailNotifications { get; set; }
        public Guid Id { get; set; }
        [Display(Name = "Heure des notifications")]
        public int HourNotifications { get; set; }
    }
}