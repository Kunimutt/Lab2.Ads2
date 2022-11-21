using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lab2.Ads2.Models
{
    public class ViewModelAdsAnnonsor
    {
        public ViewModelAdsAnnonsor() { }
        public Annons? annons { get; set; }
        public Annonsor? annonsor { get; set; }
        public List<SelectListItem> ValutorSelectList { get; set; }

    }
}
