using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace IdentitySample.Models
{
    public class RoleViewModel
    {
        public string Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "RoleName")]
        public string Name { get; set; }
    }

    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        public string Entreprise { get; set; }
        [Required]
        public string Nom { get; set; }
        [Required]
        public string Prenoms { get; set; }
        public string Adresse { get; set; }
        [Required]
        public string Ville { get; set; }
        public string Pays { get; set; }
        public string BoitePostale { get; set; }
        public string Description { get; set; }
        [Required]
        [Display(Name = "Pseudo")]
        public string UserName { get; set; }
        public IEnumerable<SelectListItem> RolesList { get; set; }
    }
}