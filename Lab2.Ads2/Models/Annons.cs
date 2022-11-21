
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lab2.Ads2.Models
{
    public class Annons
    {
        public Annons() { }
        [Required]
        [DisplayName("Rubrik")]
        public string? ad_rubrik { get; set; }
        [Required]
        [DisplayName("Innehåll")]
        public string? ad_innehall { get; set; }

        public int ad_pris { get; set; }
        [Required]
        [DisplayName("Organisationsnummer")]
        public int ad_orgnr { get; set; }
        [Required]
        [DisplayName("Prenumerationsnummer")]
        public int ad_prenr { get; set; }
        [Required]
        [DisplayName("Varans eller tjänstens pris")]
        public int ad_varpris { get; set; }

        [Required]
        [DisplayName("Segment")]
        public bool? al_foretag { get; set; }
        [Required]
        [DisplayName("Valuta")]
        public string ad_valuta { get; set; }

    }
}
