using Blazored.Modal;
using Blazored.Modal.Services;
using Blazored.Toast.Services;
using FOBlazorComponents.Helpers;
using FOBlazorEscalasManager.Data;
using FOBlazorEscalasManager.Models;
using FOBlazorEscalasManager.Services;
using FOBlazorEscalasManager.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceDocPortuaria;
using System.Security.Cryptography.X509Certificates;

namespace FOBlazorEscalasManager.Pages.Atraques
{
    public partial class IndiceAtraque : ComponentBase
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
        public string Barco { get; set; }

        [Parameter]
        public decimal id { get; set; }
       

        //Listas
        PagedResult<Atraque> listaAtraques = new PagedResult<Atraque>();
        List<Atraque> atraques = new List<Atraque>();

        //Clases
        //protected Barco barco = new Barco();
       // protected Puerto puerto = new Puerto();
        protected Atraque AtraqueEliminar = new Atraque();
        protected Escala escala = new Escala();

        //Variables
        bool renderizado = false;
        protected int buscarElemento=0;

        private DocPortuariaSoapClient miservicio = new DocPortuariaSoapClient(DocPortuariaSoapClient.EndpointConfiguration.DocPortuariaSoap);


        protected override async Task OnInitializedAsync()
        {
            try
            {
                renderizado = true;
                listaAtraques.PageSize = 10;
                listaAtraques.CurrentPage = 1;
                await ActualizaDatos();
                renderizado = false;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"IndiceAtraques.OnInitializedAsync: {ex.Message}");
            }
        }

        private async Task ActualizaDatos()
        {
            try
            {

                listaAtraques = await DataContext.Atraques
                               
                               .Where(p => p.ID == id && p.N_Atraque == buscarElemento
                               || p.ID == id && buscarElemento == 0)
                               .OrderBy(p => p.Ocupacíon)
                               .GetPaged(listaAtraques.CurrentPage, listaAtraques.PageSize);


                atraques = listaAtraques.Results.ToList();

                //barco = await DataContext.Barcos
                //            .FirstOrDefaultAsync(x => x.Codigo == Barco);

                //puerto = await DataContext.Puertos
                //            .FirstOrDefaultAsync(x => x.Codigo == Puerto);

                escala = await DataContext.Escalas
                            .Include(x => x.Barco)
                            .Include(x => x.PuertoOperacion)
                            .FirstOrDefaultAsync(x => x.NumeroEscala == Escala
                                    && x.AnnoEscala == Año
                                    && (string.IsNullOrEmpty(Puerto) || x.Puerto == Puerto));

                StateHasChanged();


            }
            catch (Exception ex)
            {
                Console.WriteLine($"IndiceAtraques.ActualizaDatos: {ex.Message}");

            }
        }
        //Cambia la Página de la Paginación
        protected async Task CambiaPagina(int pagina)
        {
            try
            {
                listaAtraques.CurrentPage = pagina;
                await ActualizaDatos();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"IndiceAtraques.CambiaPagina: {ex.Message}");
            }
        }
        // Permite buscar una escala a través del input del buscador.
        protected async Task BuscarElemento(ChangeEventArgs busqueda)
        {
            try
            {
                buscarElemento = 0;
                if (busqueda.Value.ToString() != "")
                {
                    buscarElemento = Convert.ToInt32(busqueda.Value);

                }
                listaAtraques.CurrentPage = 1;
                await ActualizaDatos();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"IndiceAtraques.BuscarElemento: {ex.Message}");
            }
        }
        protected void AbreFormulario()
        {
            try
            {
                UriHelper.NavigateTo($"/Atraques/Editar/" + id + "/0/0");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"IndiceAtraques.AbreFormulario: {ex.Message}");
            }
        }

        protected void EditarAtraque(decimal Id, int N_Atraque, int Editar)
        {
            try
            {
                UriHelper.NavigateTo($"/Atraques/Editar/" + Id + "/" + N_Atraque + "/" + Editar );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"IndiceAtraques_EditarAtraque: {ex.Message}");
            }
        }
        protected void EliminarAtraque(Atraque registro)
        {
            try
            {
                if (registro.N_Atraque > 1)
                {
                    var parameter = new ModalParameters();
                    parameter.Add("Dialogo", "¿Está seguro que desea cancelar el atraque Nº " + registro.N_Atraque + " ?");

                    AtraqueEliminar = registro;
                    Modal.OnClose += ModalClosed;
                    Modal.Show<CancelarEscala>("Cancelar Atraque", parameter);
                }
                else
                {
                    ToastService.ShowError("No se puede eliminar el primer atraque.", "Error");
                }        

            }

            catch (Exception ex)
            {
                Console.WriteLine($"IndiceAtraques.EliminarAtraque: {ex.Message}");
                ToastService.ShowError("Los servidores no se encuentran disponibles. Por favor, inténtelo de nuevo más tarde.", "Error");
            }
        }
        //Elimina el registro seleccionado
        protected async Task EliminarRegistro()
        {
            try
            {

                AtraqueEliminar.Estado = "BTR";
                
                //El primer atraque no se puede eliminar
                if (AtraqueEliminar.N_Atraque > 1)
                {
                                                                     
                    if (escala.PuertoOperacion.Autonomico == 0 && escala.NumeroEscala < 100000)
                    {

                        //Mensaje de Cancelación a Puertos del Estado

                        ToastService.ShowWarning("Enviando cancelación a Puertos del Estado.", "Espere...");
                        
                        //Cancelación de atraque     
                        await miservicio.NuevoAltaBermanAsync(escala.NumeroEscala, escala.AnnoEscala, escala.Puerto, true, 40, 
                                AtraqueEliminar.N_Atraque,AtraqueEliminar.TipoAtraque, Convert.ToDateTime(AtraqueEliminar.Ocupacíon), Convert.ToDateTime(AtraqueEliminar.Desocupacíon));
                      
                        
                    }

                    DataContext.Atraques.Remove(AtraqueEliminar);
                    await DataContext.SaveChangesAsync();
                    await ActualizaDatos();

                    //Reemplazo Muelle, Estado, Practicaje, ETA y ETD en la escala con el último atraque

                    //Muelle
                    escala.Muelle = atraques.LastOrDefault().Muelle_ID;
                    //Estado
                    escala.Estado = AtraqueEliminar.Estado;
                    //ETD
                    escala.FechaDesatraque = System.Convert.ToDateTime(String.Format("{0:d}", atraques.LastOrDefault().Desocupacíon));
                    escala.HoraDesatraque = System.Convert.ToDateTime(String.Format("{0:t}", atraques.LastOrDefault().Desocupacíon));
                    //ETA
                    escala.FechaAtraque = System.Convert.ToDateTime(String.Format("{0:d}", atraques.FirstOrDefault().Ocupacíon));
                    //escala.HoraAtraque = System.Convert.ToDateTime(String.Format("{0:t}", atraques.FirstOrDefault().Ocupacíon));
                    escala.HoraLLegada = System.Convert.ToDateTime(String.Format("{0:t}", atraques.FirstOrDefault().Ocupacíon));
                    //Practicaje
                    escala.Practicaje = atraques.LastOrDefault().Praticaje;

                    await DataContext.SaveChangesAsync();
                    await ActualizaDatos();

                    if (escala.PuertoOperacion.Autonomico == 0 && escala.NumeroEscala < 100000)
                    {
                        //Mensaje de Modificación de ETA a Puertos del estado
                        await miservicio.AltaBermanAsync(escala.NumeroEscala, escala.AnnoEscala, escala.Puerto, true, 54, Convert.ToInt32(escala.Practicaje),0);

                    }

                    ToastService.ShowSuccess("El atraque se ha eliminado.", "Correcto");
                   
                }

            }

            catch (Exception ex)
            {
                Console.WriteLine($"IndiceAtraques.EliminarRegistro: {ex.Message}");
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
                Console.WriteLine($"IndiceAtraques.ModalClosed: {ex.Message}");
            }

        }
       
    }
}