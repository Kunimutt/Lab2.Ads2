using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lab2.Ads2.Models
{
    public class ViewModelAdsPren
    {
        public ViewModelAdsPren() { }

        public Annons? annons { get; set; }
        public Pren? pren { get; set; }
        public List<SelectListItem> ValutorSelectList { get; set; }
    }
}


