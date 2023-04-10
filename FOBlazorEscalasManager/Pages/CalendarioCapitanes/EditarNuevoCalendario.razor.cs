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
using Microsoft.AspNetCore.Components.Forms;

namespace FOBlazorEscalasManager.Pages.CalendarioCapitanes
{
    public partial class EditarNuevoCalendario : ComponentBase
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

        [Parameter]
        public int EditarNuevo { get; set; }

        [Parameter]
        public decimal Id { get; set; }

             
        //Lista
        List<Barco> listaBarcos = new List<Barco>();
        List<Capitan> listaCapitanes = new List<Capitan>();

        // Clases
        protected CalendarioCapitan calendario = new CalendarioCapitan();
        public FODataGrid<Barco> componenteBarco; //Referencia
        public FODataGrid<Capitan> componenteCapitan; //Referencia

        // Variables
        protected bool frmIncompleto = false;
        protected bool practicaje=false;
        string descripcionbuque = string.Empty;



        protected override async Task OnInitializedAsync()
        {
            try
            {
                await ActualizaDatos();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EditarNuevoCalendario.OnInitializedAsync: {ex.Message}");
            }
        }
        private async Task ActualizaDatos()
        {
            try
            {
                listaBarcos = await DataContext.Barcos                                  
                                  .OrderBy(x => x.Descripcion)
                                  .ToListAsync();

                //componenteBarco.ActualizarLista(listaBarcos);
               

                listaCapitanes = await DataContext.Capitanes
                                  .Where(p => p.Baja == 0)
                                  .OrderBy(x => x.Nombre)
                                  .ToListAsync();

                //componenteCapitan.ActualizarLista(listaCapitanes);
               

                if (Id != 0)
                {
                    calendario = await DataContext.CalendarioCapitanes
                                                    .FirstOrDefaultAsync(x => x.Id == Id);
                    if (calendario.Practicaje == 0) { practicaje = true; }
                }
               
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EditarNuevoCalendario.ActualizaDatos: {ex.Message}");
                ToastService.ShowError("Los servidores no se encuentran disponibles. Por favor, inténtelo de nuevo más tarde.", "Error");
            }
        }
        //Guardar
        protected async Task Guardar()
        {
          
                if (EditarNuevo == 0)
                {
                    await AgregarRegistro();
                }
                else
                {
                    await EditarRegistro();
                }
         
               
        }
        //Actualiza el registro seleccionado
        protected async Task EditarRegistro()
        {
            try
            {
                if (!string.IsNullOrEmpty(calendario.Buque))
                {
                    var editarcalendario = await DataContext.CalendarioCapitanes
                                                    .FirstOrDefaultAsync(x => x.Id == Id);

                    editarcalendario = calendario;

                    editarcalendario.Practicaje = 0;
                    editarcalendario.Buque = calendario.Buque;
                    editarcalendario.Capitan = calendario.Capitan;
                    editarcalendario.Al_Mando_Desde = calendario.Al_Mando_Desde;
                    editarcalendario.Al_Mando_Hasta = calendario.Al_Mando_Hasta;
                    if (!practicaje) { editarcalendario.Practicaje = 1; }

                    await DataContext.SaveChangesAsync();

                    ToastService.ShowSuccess("El registro se actualizó correctamente.", "Correcto");
                    StateHasChanged();
                    AccederPaginaInicial();
                }
                else
                {
                    frmIncompleto = true;
                    ToastService.ShowError("Por favor, introduzca los campos obligatorios.", "Error");
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"EditarNuevoCalendario.EditarRegistro: {ex.Message}");
                ToastService.ShowError("Los servidores no se encuentran disponibles. Por favor, inténtelo de nuevo más tarde.", "Error");
            }
        }

        // Inserta un nuevo registro
        protected async Task AgregarRegistro()
        {
            try
            {
               
                await ActualizaDatos();

                calendario.Practicaje = 0;
                if (!practicaje) { calendario.Practicaje = 1; }

                await DataContext.CalendarioCapitanes.AddAsync(calendario);
                await DataContext.SaveChangesAsync();

                ToastService.ShowSuccess("El registro se almacenó correctamente.", "Correcto");
                await ActualizaDatos();
                StateHasChanged();

                InicializaDatos();
                //AccederPaginaInicial();
               
            }

            catch (Exception ex)
            {
                Console.WriteLine($"EditarNuevoCalendario.AgregarRegistro: {ex.Message}");
                ToastService.ShowError("Los servidores no se encuentran disponibles. Por favor, inténtelo de nuevo más tarde.", "Error");
            }
        }

        protected void InicializaDatos ()
        {
            //Inicializa datos
            var fechaDesde = calendario.Al_Mando_Hasta.Value.AddMinutes(1);
            var barco = calendario.Buque;

            calendario = new CalendarioCapitan();
            calendario.Buque = barco;
            calendario.Al_Mando_Desde = fechaDesde;
            calendario.Al_Mando_Hasta = null;
            practicaje = true;
            
        }

        protected void AccederPaginaInicial()
        {
            try
            {
                UriHelper.NavigateTo($"/Capitanes/Calendario/");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EditarNuevoCalendario.AccederPaginaInicial: {ex.Message}");
            }
        }

        private void SaveDate_Desde(DateTime fecha )
        {

            try
            {
                calendario.Al_Mando_Desde = fecha;

            }
            catch (Exception ex)
            {

                Console.WriteLine($"EditarNuevoCalendario.SaveDate_Desde: {ex.Message}");
            }
        }

        private void SaveDate_Hasta(DateTime fecha)
        {

            try
            {
                calendario.Al_Mando_Hasta = fecha;

            }
            catch (Exception ex)
            {

                Console.WriteLine($"EditarNuevoCalendario.SaveDate_Hasta: {ex.Message}");
            }
        }

        private async Task AceptarBarco(string Codigo)
        {

            try
            {
                //descripcionbuque = listaBarcos.FirstOrDefault(x => x.Codigo == Codigo);
                calendario.Buque = Codigo;
                await ActualizaDatos();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"IndiceEscalas.AceptarBarco: {ex.Message}");
            }
        }
        private async Task AceptarCapitan(string Nombre)
        {

            try
            {
                calendario.Capitan = Nombre;
                await ActualizaDatos();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"IndiceEscalas.AceptarCapitan: {ex.Message}");
            }
        }

    }
}
