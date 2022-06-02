using Nest_Homework_Partial.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nest_Homework_Partial.ViewModels
{
    public class HomeVM
    {
        public List<Slider> Sliders { get; set; }
        public List<Category> Categories { get; set; }
        //public List<Product> Products { get; set; }bu artiq lazim deyil
        public List<Product> RecentProducts { get; set; }
        public List<Product> TopRatedProducts { get; set; }
        public List<Setting> Settings { get; set; }
    }
}
