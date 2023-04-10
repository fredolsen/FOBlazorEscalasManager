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


namespace FOBlazorEscalasManager.Pages.Capitanes
{
    public partial class IndiceCapitan :ComponentBase

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

        //Lista
        //private List<Capitan> listaCapitanes = new List<Capitan>();
        PagedResult<Capitan> listaCapitanes = new PagedResult<Capitan>();
        List<Pais> listapaises = new List<Pais>();

        //Clases
        protected Capitan capitanEliminar = new Capitan();

        //Variables
        protected string buscarElemento = string.Empty;
        bool renderizado = false;


        protected override async Task OnInitializedAsync()
        {
            try
            {
                renderizado = true;
                listaCapitanes.PageSize = 20;
                listaCapitanes.CurrentPage = 1;
                await ActualizaDatos();
                renderizado = false;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"ListadoCapitanes.OnInitializedAsync: {ex.Message}");
            }
        }

        private async Task ActualizaDatos()
        {
            try
            {
                listaCapitanes = await DataContext.Capitanes
                                .Include(x => x.Pais)
                                .Where(p => p.Nombre.Contains(buscarElemento)
                                        && p.Baja == 0)                                
                                .OrderBy(p => p.Nombre)                             
                                .GetPaged(listaCapitanes.CurrentPage, listaCapitanes.PageSize);

                //listapaises = await DataContext.Paises
                //                .OrderBy(p => p.Descripcion)
                //                .ToListAsync();

                StateHasChanged();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"ListadoCapitanes.ActualizaDatos: {ex.Message}");

            }
        }
        //Cambia la Página de la Paginación
        protected async Task CambiaPagina(int pagina)
        {
            try
            {
                listaCapitanes.CurrentPage = pagina;
                await ActualizaDatos();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ListadoCapitanes.CambiaPagina: {ex.Message}");
            }
        }

        // Abre el formulario capitán
        protected void AbreFormulario(decimal Id, int editar)
        {
            try
            {
                UriHelper.NavigateTo($"/Capitanes/EditarNuevo/" + Id + "/" + editar);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ListadoCapitanes.AbreFormulario: {ex.Message}");
            }
        }

        //Elimina el registro seleccionado
        protected async Task EliminarRegistro()
        {
            try
            {
                

                DataContext.Capitanes.Remove(capitanEliminar);
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

        protected void EliminarCapitan(Capitan registro )
        {
            try
            {
                capitanEliminar = registro;
                Modal.OnClose += ModalClosed;
                Modal.Show<EliminarElemento>("Eliminar capitán");
            }

            catch (Exception ex)
            {
                Console.WriteLine($"ListadoCapitanes.EliminarCapitan: {ex.Message}");
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
                Console.WriteLine($"ListadoCapitanes.ModalClosed: {ex.Message}");
            }
        }

        // Permite buscar a un capitán a través del input del buscador.
        protected async Task BuscarElemento(ChangeEventArgs busqueda)
        {
            try
            {
                buscarElemento = busqueda.Value.ToString();
                listaCapitanes.CurrentPage = 1;
                await ActualizaDatos();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ListadoCapitanes.BuscarElemento: {ex.Message}");
            }
        }

    }


}

