using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lab2.Ads2.Models
{
    public class Pren
    {
        public Pren() { }

        [Required]
        [DisplayName("Prenumerationsnummer")]
        public int pr_prennr { get; set; }
        [Required]
        [DisplayName("Förnamn")]
        public string? pr_fnamn { get; set; }
        [Required]
        [DisplayName("Telefonnummer")]
        public string? pr_telefonnr { get; set; }
        [Required]
        [DisplayName("Utdelningsadress")]
        public string? pr_utadress { get; set; }
        [Required]
        [DisplayName("Postnummer")]
        public int pr_postnr { get; set; }
        [Required]
        [DisplayName("Ort")]
        public string? pr_ort { get; set; }
        [Required]
        [DisplayName("Personummer")]
        public string? pr_personnr { get; set; }
        [Required]
        [DisplayName("Efternamn")]
        public string? pr_efternamn { get; set; }
    }
}
