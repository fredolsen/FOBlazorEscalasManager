using System.Collections.Generic;

namespace FOBlazorEscalasManager.Models
{
    public class RouteItem
    {
        public RouteItem()
        {
            Roles = new List<RolEnum>();
            RouteItems = new List<RouteItem>();
        }

        public string Title { get; set; }
        public string Route { get; set; }
        public bool Collapse { get; set; }
        public string Icono { get; set; }
        public bool IsMenu { get; set; }
        public bool IsMobile { get; set; }
        public ICollection<RolEnum> Roles { get; set; }
        public ICollection<RouteItem> RouteItems { get; set; }
    }
}
