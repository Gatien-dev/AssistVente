using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AssistVente.Models
{
    public class Operation
    {
        public Guid Id { get; set; }
        [DisplayFormat(DataFormatString = "{0:### ### ### ### ### ###}")]
        public double Montant { get; set; }
        [DataType(DataType.Date)]
        [Display(Name ="Date d'enregistrement")]
        public DateTime Date { get; set; }
        public string UserId { get; set; }
        public Guid? ClientId { get; set; }
        public virtual Client Client { get; set; }
    }
}