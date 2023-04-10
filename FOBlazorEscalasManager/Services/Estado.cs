using FOBlazorEscalasManager.Models;
using System.Collections.Generic;

namespace FOBlazorEscalasManager.Services
{
    public class Estado
    {
        public AuthModel Token { get; set; }

        public ICollection<RouteItem> RouteItems
        {
            get
            {
                return new List<RouteItem>
                {
                    new RouteItem { Title = "Inicio", IsMenu = true, Route = "/", Icono = "fas fa-home", Roles = new List<RolEnum> {  } },
                     new RouteItem { Title = "Escalas", IsMenu = true, Route = "/Escalas", Icono = "fas fa-anchor", Roles = new List<RolEnum> {  } },
                     new RouteItem { Title = "Atraques", IsMenu = false, Route = "/Atraques", Icono = "fas fa-home", Roles = new List<RolEnum> {  },
                     
                    },
                     new RouteItem { Title = "Capitanes", IsMenu = true, Route = "/", Icono = "fas fa-user-tie", Roles = new List<RolEnum> {  },
                        RouteItems = new List<RouteItem>
                        {
                            new RouteItem { Title = "Listado", IsMenu = true, IsMobile = false, Route = "/Capitanes/Listado", Icono = "fas fa-clipboard-list", Roles = new List<RolEnum> {  } },
                            new RouteItem { Title = "Calendario", Route = "/Capitanes/Calendario", IsMenu = true, IsMobile = false, Icono = "far fa-calendar-alt", Roles = new List<RolEnum> { }},
                        }

                    },
                     new RouteItem { Title = "Enlaces", IsMenu = true, Route = "/EnlacesInteres", Icono = "fas fa-link", Roles = new List<RolEnum> {  },
                     },

                };
            }
        }

        public bool SavedMessage { get; set; }
        public bool MobileVersion { get; set; }

    }
}
