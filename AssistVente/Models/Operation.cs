using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssistVente.Models
{
    public class Operation
    {
        public Guid Id { get; set; }
        public double Montant { get; set; }
        public DateTime Date { get; set; }
        public string UserId { get; set; }
    }
}