using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lab2.Ads2.Models
{
    public class Annonsor
    {
        public Annonsor() { }

        [Required]
        [DisplayName("Organisationsnummer")]
        public int an_orgnr { get; set; }
        [Required]
        [DisplayName("Förtagsnamn")]
        public string? an_namn { get; set; }
        [Required]
        [DisplayName("Telefonnummer")]
        public string an_telnr { get; set; }
        [Required]
        [DisplayName("Utdelningsadress")]
        public string an_utadress { get; set; }
        [Required]
        [DisplayName("Postnummer")]
        public int an_postnr { get; set; }
        [Required]
        [DisplayName("Ort")]
        public string an_ort { get; set; }
        [Required]
        [DisplayName("Adress")]
        public string an_f_utadress { get; set; }
        [Required]
        [DisplayName("Postnummer")]
        public int an_f_postnr { get; set; }
        [Required]
        [DisplayName("Ort")]
        public string an_f_ort { get; set; }
    }
  
}
