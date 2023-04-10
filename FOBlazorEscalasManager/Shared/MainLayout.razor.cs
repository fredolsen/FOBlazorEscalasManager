using FOBlazorComponents.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using FOBlazorEscalasManager.Models;
using FOBlazorEscalasManager.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FOBlazorEscalasManager.Shared
{
    public partial class MainLayout : LayoutComponentBase
    {
        private const int MOBILE_WIDTH = 576;
        private const string APP_NAME = "Escalas Manager";

        [Inject]
        private AuthService AuthService { get; set; }

        [Inject]
        private Estado Estado { get; set; }

        [Inject]
        private IJSRuntime JsRuntime { get; set; }

        private bool loading = true;
        private bool collapseNavMenu = false;

        private string ActiveClass => collapseNavMenu ? "active" : null;

        private ICollection<MenuItem> MenuItems = new Collection<MenuItem>();
        private ICollection<MenuItem> NavMenuItems = new Collection<MenuItem>();
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {

                if (Estado.Token == null)
                {
                    await AuthService.Autorizar();
                }

                if (Estado.Token != null)
                {
                    if (MenuItems != null)
                    {
                        MenuItems.Clear();
                    }
                   
                }

                MenuItems = FiltraMenuItems(Estado.RouteItems, MenuItems);
                NavMenuItems = GetMenusNavegacion();

                loading = false;
                StateHasChanged();
            }
        }

        private void ActualizaMenus()
        {
            if (!string.IsNullOrEmpty(Estado.Token.ToString()))
            {
                if (MenuItems != null)
                {
                    MenuItems.Clear();
                }
                MenuItems = FiltraMenuItems(Estado.RouteItems, MenuItems);
                NavMenuItems = GetMenusNavegacion();
            }

            loading = false;
            StateHasChanged();
        }

        private ICollection<MenuItem> FiltraMenuItems(ICollection<RouteItem> routeItems, ICollection<MenuItem> menuItems)
        {
            foreach (var item in routeItems.Where(x => x.IsMenu))
            {
                var currentItem = item;
                if (item.Roles.Count() == 0 || item.Roles.Contains(Estado.Token.Usuario.Rol))
                {
                    var nuevoMenu = new MenuItem
                    {
                        Icon = item.Icono,
                        Route = item.Route,
                        Collapse = item.Collapse,
                        Title = item.Title
                    };

                    if (item.RouteItems != null)
                    {
                        FiltraMenuItems(item.RouteItems, nuevoMenu.MenuItems);
                    }

                    menuItems.Add(nuevoMenu);
                }
            }
            return menuItems;
        }

        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
            StateHasChanged();
        }

        private async Task ToggleNavMenuSideBar()
        {
            var width = await JsRuntime.InvokeAsync<int>("GetWiewPortWidth");
            if (width < MOBILE_WIDTH)
            {
                collapseNavMenu = !collapseNavMenu;
                StateHasChanged();
            }
        }

        private async Task LogOut()
        {
            await AuthService.CerrarSesion();
            StateHasChanged();
        }

        private async Task Login(LoginModel loginModel)
        {
            var error = await AuthService.Login(loginModel.Username, loginModel.Password);
            if (Estado.Token == null)
            {
                loginModel.ErrorMessage = error;
            }
            StateHasChanged();
        }

        private async Task NavMenuSelected(string menuTitle)
        {
            if (menuTitle == "Cerrar sesión")
            {
                await AuthService.CerrarSesion();
                StateHasChanged();
            }
        }

        private ICollection<MenuItem> GetMenusNavegacion()
        {
            return new List<MenuItem>
            {
                new MenuItem { Title = "Cerrar sesión", Icon = "fas fa-power-off" }
            };
        }
    }
}
