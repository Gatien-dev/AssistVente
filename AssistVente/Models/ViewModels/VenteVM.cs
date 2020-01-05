﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssistVente.Models.ViewModels
{
    public class VenteVM
    {
        public Guid ClientId { get; set; }
        public List<DetailVenteVM> Details { get; set; }
    }
    public class DetailVenteVM
    {
        public Guid ProduitId { get; set; }
        public string NomProduit { get; set; }
        public double PU { get; set; }
        public double Quantite { get; set; }
    }
}