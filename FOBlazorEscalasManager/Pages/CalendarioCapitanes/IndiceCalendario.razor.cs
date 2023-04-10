using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using FOBlazorEscalasManager.Data;
using FOBlazorEscalasManager.Models;
using FOBlazorComponents;
using FOBlazorEscalasManager.Services;
using Blazored.Toast.Services;
using Microsoft.EntityFrameworkCore;
using FOBlazorComponents.Helpers;
using Blazored.Modal.Services;
using FOBlazorEscalasManager.Shared;

namespace FOBlazorEscalasManager.Pages.CalendarioCapitanes
{
    public partial class IndiceCalendario: ComponentBase
    {
        [Inject]
        DataContext DataContext { get; set; }

        [Inject]
        protected Estado Estado { get; set; }

        [Inject]
        NavigationManager UriHelper { get; set; }

        [Inject]
        IToastService ToastService { get; set; }

        [Inject]
        IModalService Modal { get; set; }

        [Parameter]
        public string buscar { get; set; }

        // Lista
        PagedResult<CalendarioCapitan> listaCalendarioCapitan = new PagedResult<CalendarioCapitan>();
        List<Barco> listaBarco = new List<Barco>();

        //Clases
        protected CalendarioCapitan calendarioEliminar = new CalendarioCapitan();

        //Variables
        protected string buscarElemento = string.Empty;
        bool renderizado = false;


        protected override async Task OnInitializedAsync()
        {
            try
            {
                renderizado = true;
                listaCalendarioCapitan.PageSize = 20;
                listaCalendarioCapitan.CurrentPage = 1;
                await ActualizaDatos();
                renderizado = false;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"IndiceCalendario.OnInitializedAsync: {ex.Message}");
            }
        }

        private async Task ActualizaDatos()
        {
            try
            {

                listaBarco = await DataContext.Barcos
                                   .OrderBy(x => x.Codigo)
                                   .ToListAsync();

                listaCalendarioCapitan = await DataContext.CalendarioCapitanes
                                .Where(p => p.Capitan.Contains(buscarElemento) || p.Buque.Contains(buscarElemento))
                                .OrderByDescending(p => p.Al_Mando_Hasta)
                                .GetPaged(listaCalendarioCapitan.CurrentPage, listaCalendarioCapitan.PageSize);

               

                StateHasChanged();


            }
            catch (Exception ex)
            {
                Console.WriteLine($"AgregarUsuario.OnInitializedAsync: {ex.Message}");

            }
        }

        //Cambia la Página de la Paginación
        protected async Task CambiaPagina(int pagina)
        {
            try
            {
                listaCalendarioCapitan.CurrentPage = pagina;
                await ActualizaDatos();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AdministracionEquipos.CambiaPagina: {ex.Message}");
            }
        }

        // Abre el formulario capitán
        protected void AbreFormulario(decimal Id, int editar)
        {
            try
            {
                UriHelper.NavigateTo($"/Capitanes/Calendario/EditarNuevo/" + Id + "/" + editar);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"IndiceCalendario_AbreFormulario_CalendarioCapitanes: {ex.Message}");
            }
        }

        //Elimina el registro seleccionado
        protected async Task EliminarRegistro()
        {
            try
            {


                DataContext.CalendarioCapitanes.Remove(calendarioEliminar);
                await DataContext.SaveChangesAsync();
                await ActualizaDatos();
                ToastService.ShowSuccess("El registro se eliminó correctamente.", "Correcto");
                //AccederPaginaInicial();


            }

            catch (Exception ex)
            {
                Console.WriteLine($"AgregarUsuario.AgregarUsuarios: {ex.Message}");
                ToastService.ShowError("Los servidores no se encuentran disponibles. Por favor, inténtelo de nuevo más tarde.", "Error");
            }
        }

        protected void EliminarCalendario(CalendarioCapitan registro)
        {
            try
            {
                calendarioEliminar = registro;
                Modal.OnClose += ModalClosed;
                Modal.Show<EliminarElemento>("Eliminar Calendario");
            }

            catch (Exception ex)
            {
                Console.WriteLine($"AgregarUsuario.IntermediarioEliminarUsuario: {ex.Message}");
                ToastService.ShowError("Los servidores no se encuentran disponibles. Por favor, inténtelo de nuevo más tarde.", "Error");
            }
        }

        // Función que se activa al cerrar el modal
        void ModalClosed(ModalResult modalResult)
        {
            try
            {
                if (!modalResult.Cancelled)
                {
                    if (Convert.ToBoolean(modalResult.Data))
                    {
                        EliminarRegistro();

                    }
                }

                Modal.OnClose -= ModalClosed;
            }

            catch (Exception ex)
            {
                Console.WriteLine($"AgregarUsuario.ModalClosed: {ex.Message}");
            }
        }

        // Permite buscar en el calendario a través del input del buscador.
        protected async Task BuscarElemento(ChangeEventArgs busqueda)
        {
            try
            {
                buscarElemento = busqueda.Value.ToString();
                listaCalendarioCapitan.CurrentPage = 1;

                if (busqueda.Value.ToString() != "") 
                { 
                    UriHelper.NavigateTo($"/Capitanes/Calendario/" + buscarElemento); 
                }
                else
                {              
                    await ActualizaDatos();
                    UriHelper.NavigateTo($"/Capitanes/Calendario");
                }



            }
            catch (Exception ex)
            {
                Console.WriteLine($"AdministracionEquipos.BuscarElemento: {ex.Message}");
            }
        }

        protected override void OnParametersSet()
        {
            buscarElemento = buscar;
            if (buscar != null)
            {
                //Task.Delay(2000);
                ActualizaDatos();                             
            }

           
            base.OnParametersSet();
        }

    }
}
