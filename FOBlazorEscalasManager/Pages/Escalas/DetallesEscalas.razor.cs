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
using Blazored.Modal;
using ServiceDocPortuaria;
using System.Runtime.CompilerServices;

namespace FOBlazorEscalasManager.Pages.Escalas
{
    public partial class DetallesEscalas : ComponentBase
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
       
        //Clases
        public Escala EscalaActual = new Escala();
        protected Puerto PuertoOperacion = new Puerto();
        protected Puerto Origen = new Puerto();
        protected Puerto Destino = new Puerto();
        protected Barco BuqueActual = new Barco();
        protected EstadoEscala EstadoActual = new EstadoEscala();
        bool renderizado = false;

             
        //Web Service
        private DocPortuariaSoapClient miservicio = new DocPortuariaSoapClient(DocPortuariaSoapClient.EndpointConfiguration.DocPortuariaSoap);


        protected override async Task OnInitializedAsync()
        {
            try
            {
                renderizado = true;
                await ActualizaDatos();
                renderizado = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DetallesEscalas.OnInitializedAsync: {ex.Message}");
            }
        }
        private async Task ActualizaDatos()
        {
            try
            {
               

                EscalaActual = await DataContext.Escalas  
                                    .Include(x => x.Error)
                                    .FirstOrDefaultAsync(x => x.NumeroEscala == Escala
                                                        && x.AnnoEscala == Año
                                                        && (string.IsNullOrEmpty(Puerto) || x.Puerto == Puerto));

               
                PuertoOperacion = await DataContext.Puertos
                                .FirstOrDefaultAsync(x => x.Codigo == Puerto || string.IsNullOrEmpty(Puerto));

                Origen = await DataContext.Puertos
                                .FirstOrDefaultAsync(x => x.Codigo == EscalaActual.PuertoAnterior);

                Destino = await DataContext.Puertos
                                .FirstOrDefaultAsync(x => x.Codigo == EscalaActual.PuertoSiguiente);

                BuqueActual = await DataContext.Barcos
                               .FirstOrDefaultAsync(x => x.Codigo == EscalaActual.Buque);

                EstadoActual = await DataContext.EstadoEscalas
                               .FirstOrDefaultAsync(x => x.EST_CCOEstado == EscalaActual.Estado);




                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DetallesEscalas.ActualizaDatos: {ex.Message}");
                ToastService.ShowError("Los servidores no se encuentran disponibles. Por favor, inténtelo de nuevo más tarde.", "Error");
            }
        }

        protected void EliminarEscala(Escala registro)
        {
            try
            {
                //renderizado = true;
                var parameter = new ModalParameters();
                parameter.Add("Dialogo", "¿Está seguro que desea cancelar esta escala?");

                EscalaActual = registro;
                Modal.OnClose += ModalClosed;
                Modal.Show<CancelarEscala>("Cancelar escala", parameter);
                              
            }

            catch (Exception ex)
            {
                Console.WriteLine($"DetallesEscalas.EliminarEscala: {ex.Message}");
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
                Console.WriteLine($"DetallesEscalas.ModalClosed: {ex.Message}");
            }

        }
        protected async Task EliminarRegistro()
        {
            try
            {

                renderizado = true;
                EscalaActual.Estado = "ASC";
                EscalaActual.Enviada = -1;

                Puerto PuertoEscala = new Puerto();
                PuertoEscala = await DataContext.Puertos
                                    .FirstOrDefaultAsync(x => x.Codigo == EscalaActual.Puerto);
                if (PuertoEscala.Autonomico == -1 || PuertoEscala.Autonomico == 1 || EscalaActual.NumeroEscala >= 100000)
                {
                    DataContext.Escalas.Remove(EscalaActual);

                }
                else
                {
                    StateHasChanged();
                    //ToastService.ShowWarning("Enviando la cancelación de escala a Puertos del Estado.", "Espere...");
                    await miservicio.AltaBermanAsync(EscalaActual.NumeroEscala, EscalaActual.AnnoEscala, EscalaActual.Puerto, true, 1, 1,0);

                }

                await DataContext.SaveChangesAsync();
                renderizado = false;

                await ActualizaDatos();                          

                ToastService.ShowSuccess("La escala se ha cancelado.", "Correcto");
                //AccederPaginaInicial();
               
               
               

            }

            catch (Exception ex)
            {
                Console.WriteLine($"DetallesEscalas.EliminarRegistro: {ex.Message}");
                ToastService.ShowError("Los servidores no se encuentran disponibles. Por favor, inténtelo de nuevo más tarde.", "Error");
            }
        }
        protected void EditarEscala(string Puerto, Int32 Escala, Int32 Año, int editar)
        {
            try
            {
                UriHelper.NavigateTo($"/Escalas/Editar/" + Puerto + "/" + Escala + "/" + Año + "/1");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DetallesEscalas: {ex.Message}");
            }
        }
        protected void Atraques(string Puerto, Int32 Escala, Int32 Año, string Barco, decimal id)
        {
            try
            {
                UriHelper.NavigateTo($"/Atraques/" + id + "/" + Escala + "/" + Año + "/" + Puerto + "/" + Barco );
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DetallesEscalas: {ex.Message}");
            }
        }

        protected void AccederPaginaInicial()
        {
            try
            {
                UriHelper.NavigateTo($"/Escalas");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DetallesEscalas.AccederPaginaInicial: {ex.Message}");
            }
        }
        protected void DeclaracionSumaria()
        {
            try
            {
                UriHelper.NavigateTo($"/Declaraciones/" + Puerto + "/" + Escala + "/" + Año + "/D");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"DetallesEscalas: {ex.Message}");
            }
        }
        protected void ManifiestoCarga()
        {
            try
            {
                UriHelper.NavigateTo($"/Declaraciones/" + Puerto + "/" + Escala + "/" + Año + "/C");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"DetallesEscalas: {ex.Message}");
            }
        }

    }
}
