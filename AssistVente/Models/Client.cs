using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AssistVente.Models
{
    public class Client
    {
        public Guid ID { get; set; }
        [Display(Name = "Nom complet")]
        public string Nom { get; set; }
        [Display(Name = "Date de naissance")]
        [DataType(DataType.Date)]
        public DateTime DateNaissance { get; set; }
        [Display(Name = "Lieu de naissance")]
        public string LieuNaiss { get; set; }
        public bool Default { get; set; }
        public string Adresse { get; set; }
        [Display(Name = "Téléphone")]
        public string Telephone { get; set; }
        public string Email { get; set; }
        [Display(Name = "Nom complet")]
        public string NomUrgence { get; set; }
        [Display(Name = "Addresse")]
        public string AddresseUrgence { get; set; }
        [Display(Name = "Téléphone")]
        public string TelephoneUrgence { get; set; }
        [Display(Name = "Photos autorisées")]
        public bool PhotosAllowed { get; set; }
        [Display(Name = "Certificat Medical")]
        public bool CertficatMedical { get; set; }

    }
}