using Blazored.Modal.Services;
using Blazored.Toast.Services;
using FOBlazorEscalasManager.Data;
using FOBlazorEscalasManager.Models;
using FOBlazorEscalasManager.Services;
using Microsoft.AspNetCore.Components;
using FOBlazorComponents.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System.Security.Cryptography.X509Certificates;

namespace FOBlazorEscalasManager.Pages.Declaraciones
{
    public partial class IndiceDeclaraciones : ComponentBase
    {

        [Inject]
        DataContext DataContext { get; set; }

        [Inject]
        NavigationManager UriHelper { get; set; }

        [Inject]
        protected Estado Estado { get; set; }

        [Inject]
        IToastService ToastService { get; set; }

        [Inject]
        IModalService Modal { get; set; }

        //Parametros
        [Parameter]
        public Int32 Escala { get; set; }
        [Parameter]
        public Int32 Año { get; set; }
        [Parameter]
        public string Puerto { get; set; }
        [Parameter]
        public string Actividad { get; set; }

        //Clase
        private Escala escala = new Escala();
        protected EstadoEscala EstadoActual = new EstadoEscala();
        //public class matricula
        //{
        //    public string Matricula { get; set; }
        //    public matricula()
        //    {
        //    }

        //    public matricula(string Matricula)
        //    {
        //        this.Matricula = Matricula;
        //    }
        //}

        //Listas
        protected PagedResult<Manifiesto> listamanifiestos = new PagedResult<Manifiesto>();
        protected List<Puerto> listaPuertos = new List<Puerto>();
        protected List<Manifiesto> lista = new List<Manifiesto>();
        protected List<TipoPartida> listatipopartida = new List<TipoPartida>();
        protected List<TipoVehiculos> listatipovehiculo = new List<TipoVehiculos>();


        //Variables
        bool renderizado = false;
        string TipoOperacion = "Declaración Sumaria";
        string puertoOrigenSeleccionado = null;
        string puertoDestinoSeleccionado = null;
        string MatriculaSeleccionada = null;
        string MercanciaSeleccionada = null;
        string EstadoDeclaracion =null;
        string TipoPartidaSeleccionada = null;
        string TipoVehiculoSeleccionado = null;
        string Origen = string.Empty;
        string Destino = string.Empty;
        protected override async Task OnInitializedAsync()
        {
            try
            {
               
                if (Actividad == "C") { TipoOperacion = "Manifiesto de Carga"; }
                renderizado = true;
                listamanifiestos.PageSize = 20;
                listamanifiestos.CurrentPage = 1;
                await ActualizaDatos();
                renderizado = false;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"IndiceDeclaraciones.OnInitializedAsync: {ex.Message}");
            }
        }

        private async Task ActualizaDatos()
        {
            try
            {
               
                listamanifiestos = await DataContext.Manifiestos
                                .Include(p => p.Escala).ThenInclude(p =>p.PuertoOperacion)
                                .Include(p => p.Escala).ThenInclude(p => p.Barco)

                                .Where(p => p.NumeroEscala == Escala
                                                        && p.AnnoEscala == Año
                                                        && (string.IsNullOrEmpty(Puerto) || p.Puerto == Puerto)
                                                        && p.Actividad == Actividad
                                                         && (string.IsNullOrEmpty(puertoOrigenSeleccionado)
                                                        || p.PuertoAnterior == puertoOrigenSeleccionado)
                                                            && (string.IsNullOrEmpty(puertoDestinoSeleccionado)
                                                        || p.PuertoSiguiente == puertoDestinoSeleccionado)
                                                        && (string.IsNullOrEmpty(MatriculaSeleccionada)
                                                        || p.Matricula == MatriculaSeleccionada)
                                                        && (string.IsNullOrEmpty(MercanciaSeleccionada)
                                                        || p.Mercancía == MercanciaSeleccionada)
                                                         && (string.IsNullOrEmpty(TipoPartidaSeleccionada)
                                                        || p.TipoPartida == TipoPartidaSeleccionada)
                                                         && (string.IsNullOrEmpty(TipoVehiculoSeleccionado)
                                                        || p.TipoObjeto == TipoVehiculoSeleccionado))
                               .OrderBy(p => p.Orden)
                               .GetPaged(listamanifiestos.CurrentPage, listamanifiestos.PageSize);

                if (listamanifiestos.Results.Count > 0)
                {
                    escala = listamanifiestos.Results.FirstOrDefault().Escala;

                    lista = await DataContext.Manifiestos
                                          .Where(p => p.NumeroEscala == Escala
                                                        && p.AnnoEscala == Año
                                                        && p.Puerto == Puerto
                                                        && p.Actividad == Actividad)

                                        .OrderBy(p => p.Orden)
                                        .ToListAsync();


                }



                listaPuertos = await DataContext.Puertos
                                     .OrderBy(x => x.Codigo)
                                     .ToListAsync();

                listatipopartida = await DataContext.TiposPartida
                                     .OrderBy(x => x.Tipo)
                                     .ToListAsync();

                listatipovehiculo = await DataContext.TiposVehiculo
                                    .Where(p =>p.Puerto == Puerto && p.VacioLLeno == "E")
                                    .OrderBy(x => x.Descripcion)
                                    .ToListAsync();

                EstadoActual = await DataContext.EstadoEscalas
                              .FirstOrDefaultAsync(x => x.EST_CCOEstado == escala.Estado);
                if (EstadoActual != null) { EstadoDeclaracion = EstadoActual.EST_CCOEstado; }

                if (escala !=null)
                {
                    if (escala.PuertoAnterior != null || escala.PuertoSiguiente != null)
                    {
                        Origen = listaPuertos.FirstOrDefault(p => p.Codigo == escala.PuertoAnterior).Descripcion;
                        Destino = listaPuertos.FirstOrDefault(p => p.Codigo == escala.PuertoSiguiente).Descripcion;
                    }
                    
                }


                //foreach (var item in listamanifiestos.Results.Distinct())
                //{
                //    matricula mat = new matricula();
                //    if (item.Matricula != "")
                //    {
                //        mat.Matricula = item.Matricula;
                //        listaMatricula.Add(mat);
                //    }
                //}


                StateHasChanged();


            }
            catch (Exception ex)
            {
                Console.WriteLine($"IndiceDeclaraciones.ActualizaDatos: {ex.Message}");

            }
        }
        //Cambia la Página de la Paginación
        protected async Task CambiaPagina(int pagina)
        {
            try
            {
                listamanifiestos.CurrentPage = pagina;
                await ActualizaDatos();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"IndiceDeclaraciones.CambiaPagina: {ex.Message}");
            }
        }

        private async Task AceptarPuertoOrigen(string Codigo)
        {

            try
            {
                listamanifiestos.CurrentPage = 1;
                puertoOrigenSeleccionado = Codigo;
                await ActualizaDatos();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"IndiceDeclaraciones.AceptarPuertoOrigen: {ex.Message}");
            }
        }

        private async Task AceptarPuertoDestino(string Codigo)
        {

            try
            {
                puertoDestinoSeleccionado = Codigo;
                await ActualizaDatos();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"IndiceDeclaraciones.AceptarPuertoDestino: {ex.Message}");
            }
        }

        private async Task AceptarMatricula(string Codigo)
        {

            try
            {
                listamanifiestos.CurrentPage = 1;
                MatriculaSeleccionada = Codigo;
                await ActualizaDatos();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"IndiceDeclaraciones.AceptarPuertoDestino: {ex.Message}");
            }
        }

        private async Task AceptarMercancia(string Codigo)
        {

            try
            {
                listamanifiestos.CurrentPage = 1;
                MercanciaSeleccionada = Codigo;
                await ActualizaDatos();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"IndiceDeclaraciones.AceptarPuertoDestino: {ex.Message}");
            }
        }
        private async Task AceptarTipoPartida(string Codigo)
        {

            try
            {
                listamanifiestos.CurrentPage = 1;
                TipoPartidaSeleccionada = Codigo;
                await ActualizaDatos();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"IndiceDeclaraciones.AceptarPuertoDestino: {ex.Message}");
            }
        }

        private async Task AceptarTipoVehiculo(string Codigo)
        {

            try
            {
                listamanifiestos.CurrentPage = 1;
                TipoVehiculoSeleccionado = Codigo;
                await ActualizaDatos();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"IndiceDeclaraciones.AceptarPuertoDestino: {ex.Message}");
            }
        }
    }
}
