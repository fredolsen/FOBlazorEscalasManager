using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using FOBlazorEscalasManager.Data;
using FOBlazorEscalasManager.Models;
using FOBlazorComponents;
using FOBlazorEscalasManager.Services;
using Microsoft.EntityFrameworkCore;
using FOBlazorComponents.Helpers;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Components.Forms;

namespace FOBlazorEscalasManager.Pages.Capitanes
{
    public partial class EditarNuevoCapitan : ComponentBase
    {

        [Inject]
        DataContext DataContext { get; set; }

        [Inject]
        NavigationManager UriHelper { get; set; }

        [Inject]
        protected Estado Estado { get; set; }

        [Inject]
        IToastService ToastService { get; set; }

        [Parameter]
        public int EditarNuevo { get; set; }

        [Parameter]
        public decimal Id { get; set; }

        // Clases
        protected Capitan capitan = new Capitan();

        // Variables
        protected bool frmIncompleto = false;
        protected bool baja = false;

        //Lista
        private List<Pais> listapaises = new List<Pais>();
              
        protected override async Task OnInitializedAsync()
        {
            try
            {
                await ActualizaDatos();
                                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EditarNuevoCapitan.OnInitializedAsync: {ex.Message}");
            }
        }
        private async Task ActualizaDatos()
        {
            try
            {
                if (Id != 0)
                {
                    capitan = await DataContext.Capitanes
                                                    .Include(y => y.Pais)
                                                    .FirstOrDefaultAsync(x => x.Id_Capitan == Id);
                }
                if (capitan.Baja == 1) { baja = true; }

                listapaises = await DataContext.Paises
                                .OrderBy(p => p.Descripcion)
                                .ToListAsync();

                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EditarNuevoCapitan.OnInitializedAsync: {ex.Message}");
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
               
                //var editarcapitan = await DataContext.Capitanes
                //                                .FirstOrDefaultAsync(x => x.Id_Capitan == Id);

                //editarcapitan = capitan;

                capitan.Nombre = capitan.Nombre;
                capitan.Nacionalidad = capitan.Nacionalidad.ToUpper();
                capitan.Baja = 0;
                if (baja) { capitan.Baja = 1; }

                await DataContext.SaveChangesAsync();

                ToastService.ShowSuccess("El registro se actualizó correctamente.", "Correcto");
                StateHasChanged();
                AccederPaginaInicial();
               
                
            }

            catch (Exception ex)
            {
                Console.WriteLine($"EditarNuevoCapitan.EditarRegistro: {ex.Message}");
                ToastService.ShowError("Los servidores no se encuentran disponibles. Por favor, inténtelo de nuevo más tarde.", "Error");
            }
        }

        // Inserta un nuevo registro
        protected async Task AgregarRegistro()
        {
            try
            {
                if (!string.IsNullOrEmpty(capitan.Nombre))
                {
                    await ActualizaDatos();

                    capitan.Baja = 0;
                    capitan.Nacionalidad = capitan.Nacionalidad.ToUpper();
                    capitan.Baja = 0;
                    if (baja) { capitan.Baja = 1; }


                    await DataContext.Capitanes.AddAsync(capitan);
                    await DataContext.SaveChangesAsync();

                    ToastService.ShowSuccess("El registro se almacenó correctamente.", "Correcto");
                    await ActualizaDatos();
                    StateHasChanged();
                    AccederPaginaInicial();
                }
                //else
                //{
                //    frmIncompleto = true;

                //    ToastService.ShowError("Por favor, introduzca los campos obligatorios.", "Error");
                //}
            }

            catch (Exception ex)
            {
                Console.WriteLine($"EditarNuevoCapitan.AgregarRegistro: {ex.Message}");
                ToastService.ShowError("Los servidores no se encuentran disponibles. Por favor, inténtelo de nuevo más tarde.", "Error");
            }
        }
       
        protected void AccederPaginaInicial()
        {
            try
            {
                UriHelper.NavigateTo($"/Capitanes/Listado/");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EditarNuevoCapitan.AccederPaginaInicial: {ex.Message}");
            }
        }


    }
}
