using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssistVente.Models
{
    public class Client
    {
        public Guid ID { get; set; }
        public string Nom { get; set; }
        public bool Default { get; set; }
        public double Solde { get; set; }
        public string Adresse { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        

    }
}