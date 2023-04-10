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
using System.ComponentModel;
using Blazored.Modal;
using System.Collections.ObjectModel;
using ServiceDocPortuaria;

namespace FOBlazorEscalasManager.Pages.Escalas
{
    public partial class IndiceEscalas : ComponentBase
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
       
        //Listas
        PagedResult<Escala> listaEscalas = new PagedResult<Escala>();
        List<EstadoEscala> listaEstados = new List<EstadoEscala>();
        List<Barco> listaBarcos = new List<Barco>();
        List<Puerto> listaPuertos = new List<Puerto>();

        //Clases
        protected Escala EscalaEliminar = new Escala();
        FOTableDataGrid<Barco> componenteBarco = new FOTableDataGrid<Barco>();

        //Variables
        protected string buscarElemento = string.Empty;
        string barcoSeleccionado = null;
        string estadoSeleccionado = null;
        string puertoSeleccionado = null;
        string puertoOrigenSeleccionado = null;
        string puertoDestinoSeleccionado = null;
        bool renderizado = false;
        DateTime? FechaIniBuscar; /*= new DateTime(2020, 04, 23, 0, 0, 0);*/
        DateTime? FechaFinBuscar; /*= new DateTime(DateTime.Now.Year, 12, 31, 0, 0, 0);*/
        string EscalaBuscar = string.Empty;

        private DocPortuariaSoapClient miservicio = new DocPortuariaSoapClient(DocPortuariaSoapClient.EndpointConfiguration.DocPortuariaSoap);


        protected override async Task OnInitializedAsync()
        {
            try
            {
                renderizado = true;
                listaEscalas.PageSize = 20;
                listaEscalas.CurrentPage = 1;
                await ActualizaDatos();
                renderizado = false;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"IndiceEscalas.OnInitializedAsync: {ex.Message}");
            }
        }

        private async Task ActualizaDatos()
        {
            try
            {

                if (listaEscalas != null)
                {
                    listaEscalas = await DataContext.Escalas
                        //.Include(p => p.Manifiesto)
                             .Where(p => ((p.NumeroEscala.ToString().Contains(EscalaBuscar) && p.FechaDesatraque >= FechaIniBuscar 
                                    && p.FechaDesatraque <= FechaFinBuscar) || p.NumeroEscala.ToString().Contains(EscalaBuscar) 
                                    && string.IsNullOrEmpty(FechaIniBuscar.ToString()) && string.IsNullOrEmpty(FechaFinBuscar.ToString()))
                                    && (string.IsNullOrEmpty(barcoSeleccionado)
                                        || p.Buque == barcoSeleccionado)
                                        && (string.IsNullOrEmpty(estadoSeleccionado) || p.Estado == estadoSeleccionado)
                                        && (string.IsNullOrEmpty(puertoSeleccionado)
                                        || p.Puerto == puertoSeleccionado)
                                            && (string.IsNullOrEmpty(puertoOrigenSeleccionado)
                                        || p.PuertoAnterior == puertoOrigenSeleccionado)
                                            && (string.IsNullOrEmpty(puertoDestinoSeleccionado)
                                        || p.PuertoSiguiente == puertoDestinoSeleccionado)
                                        //&& (p.FechaDesatraque >= FechaIniBuscar || string.IsNullOrEmpty(FechaIniBuscar.ToString())) 
                                        //&& (p.FechaDesatraque <= FechaFinBuscar) || string.IsNullOrEmpty(FechaFinBuscar.ToString())
                                        )

                             .OrderByDescending(p => p.FechaDesatraque).ThenByDescending(p => p.HoraDesatraque)
                             .GetPaged(listaEscalas.CurrentPage, listaEscalas.PageSize);


                    listaBarcos = await DataContext.Barcos
                                       .OrderBy(x => x.Codigo)
                                       .ToListAsync();

                    componenteBarco.ActualizarLista(listaBarcos);

                    listaPuertos = await DataContext.Puertos
                                      .OrderBy(x => x.Codigo)
                                      .ToListAsync();

                    listaEstados = await DataContext.EstadoEscalas
                                     .OrderBy(x => x.EST_Descripcion)
                                     .ToListAsync();
                }

               
                StateHasChanged();


            }
            catch (Exception ex)
            {
               Console.WriteLine($"IndiceEscalas.ActualizaDatos: {ex.Message}");

            }
        }

        //Filtro de búsqueda
        protected async Task Filtro()
        {
            try
            {
                
                listaEscalas.CurrentPage = 1;
                await ActualizaDatos();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"IndiceEscalas.Filtro: {ex.Message}");
            }
        }


        //Cambia la Página de la Paginación
        protected async Task CambiaPagina(int pagina)
        {
            try
            {
                listaEscalas.CurrentPage = pagina;
                await ActualizaDatos();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"IndiceEscalas.CambiaPagina: {ex.Message}");
            }
        }

        // Abre el formulario para editar las escalas
        protected void AbreFormulario(string Puerto, Int32 Escala, Int32 Año, int editar)
        {
            try
            {
                UriHelper.NavigateTo($"/Escalas/Editar/0/0/0/0");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"IndiceEscalas.AbreFormulario: {ex.Message}");
            }
        }

        //Elimina el registro seleccionado
        protected async Task EliminarRegistro()
        {
            try
            {

               
                EscalaEliminar.Estado = "ASC";
                EscalaEliminar.Enviada = -1;

                Puerto PuertoEscala = new Puerto();
                PuertoEscala = await DataContext.Puertos
                                    .FirstOrDefaultAsync(x => x.Codigo == EscalaEliminar.Puerto);
               
                if (PuertoEscala.Autonomico ==-1 || PuertoEscala.Autonomico==1 || EscalaEliminar.NumeroEscala>=100000)
                {
                    DataContext.Escalas.Remove(EscalaEliminar);
                   
                }
                else
                {
                    ToastService.ShowWarning("Enviando la cancelación de escala a Puertos del Estado.", "Espere...");
                    
                    await miservicio.AltaBermanAsync(EscalaEliminar.NumeroEscala, EscalaEliminar.AnnoEscala, EscalaEliminar.Puerto, true, 1, 1,0);
                                        
                }

                await DataContext.SaveChangesAsync();
                await ActualizaDatos();
                ToastService.ShowSuccess("La escala se ha cancelado.", "Correcto");

                //AccederPaginaInicial();


            }

            catch (Exception ex)
            {
                Console.WriteLine($"IndiceEscalas.EliminarRegistro: {ex.Message}");
                ToastService.ShowError("Los servidores no se encuentran disponibles. Por favor, inténtelo de nuevo más tarde.", "Error");
            }
        }

        protected void EliminarEscala(Escala registro)
        {
            try
            {
                var parameter = new ModalParameters();
                parameter.Add("Dialogo", "¿Está seguro que desea cancelar esta escala?");

                EscalaEliminar = registro;
                Modal.OnClose += ModalClosed;
                Modal.Show<CancelarEscala>("Cancelar escala", parameter);
                

            }

            catch (Exception ex)
            {
                Console.WriteLine($"IndiceEscalas.EliminarEscala: {ex.Message}");
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
                Console.WriteLine($"IndiceEscalas.ModalClosed: {ex.Message}");
            }

        }

        // Permite buscar una escala a través del input del buscador.
        protected async Task BuscarElemento(ChangeEventArgs busqueda)
        {
            try
            {
                buscarElemento = busqueda.Value.ToString();
                listaEscalas.CurrentPage = 1;
                await ActualizaDatos();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"IndiceEscalas.BuscarElemento: {ex.Message}");
            }
        }

        private async Task AceptarBarco(string Codigo)
        {

            try
            {
                listaEscalas.CurrentPage = 1;
                barcoSeleccionado = Codigo;
                await ActualizaDatos();

            }
            catch(Exception ex)
            {
                Console.WriteLine($"IndiceEscalas.AceptarBarco: {ex.Message}");
            }
        }

        private async Task AceptarEstado(string Codigo)
        {

            try
            {

                listaEscalas.CurrentPage = 1;
                estadoSeleccionado = Codigo;
                await ActualizaDatos();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"IndiceEscalas.AceptarEstado: {ex.Message}");
            }
        }

        private async Task AceptarPuerto(string Codigo)
        {

            try
            {
                listaEscalas.CurrentPage = 1;
                puertoSeleccionado = Codigo;
                await ActualizaDatos();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"IndiceEscalas.AceptarPuerto: {ex.Message}");
            }
        }

        private async Task AceptarPuertoOrigen(string Codigo)
        {

            try
            {
                listaEscalas.CurrentPage = 1;
                puertoOrigenSeleccionado = Codigo;
                await ActualizaDatos();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"IndiceEscalas.AceptarPuertoOrigen: {ex.Message}");
            }
        }
       
        private async Task AceptarPuertoDestino(string Codigo)
        {

            try
            {
                listaEscalas.CurrentPage = 1;
                puertoDestinoSeleccionado = Codigo;
                await ActualizaDatos();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"IndiceEscalas.AceptarPuertoDestino: {ex.Message}");
            }
        }

        protected void Detalles(string Puerto,Int32 Escala, Int32 Año)
        {
            try
            {
                UriHelper.NavigateTo($"/Escalas/Detalles/" + Puerto + "/" + Escala+ "/" + Año);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"IndiceEscalas_Detalles: {ex.Message}");
            }
        }
    

    }
}
