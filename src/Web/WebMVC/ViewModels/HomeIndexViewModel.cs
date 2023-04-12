using MssDevLab.Common.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MssDevLab.WebMVC.ViewModels
{
    public class HomeIndexViewModel
    {
        public HomeIndexViewModel(IEnumerable<ServiceData>? commonAds) 
        { 
            CommonAds = commonAds;
        }

        public IEnumerable<ServiceData>? CommonAds { get; set; }  

        public bool HasCommonAds => CommonAds != null && CommonAds.Any();
    }
}
