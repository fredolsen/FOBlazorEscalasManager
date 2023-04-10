using FOBlazorEscalasManager.Data;
using FOBlazorEscalasManager.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using FOBlazorEscalasManager.Services;
using Blazored.Toast.Services;
using Microsoft.EntityFrameworkCore;
using FOBlazorComponents.Helpers;
using Blazored.Modal.Services;
using FOBlazorEscalasManager.Shared;
using ServiceDocPortuaria;

namespace FOBlazorEscalasManager.Pages.Atraques
{
    public partial class EditarAtraque : ComponentBase

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
        public decimal id { get; set; }
        [Parameter]
        public int N_Atraque { get; set; }
        [Parameter]
        public int EditarNuevo { get; set; }

        //Clases
        protected Barco barco = new Barco();
        protected Puerto puerto = new Puerto();     
        protected Escala escala = new Escala();
        protected Atraque atraque = new Atraque();

        //Listas
        List<Muelle> listamuelles = new List<Muelle>();
        List<TipoAtraque> listatiposatraques = new List<TipoAtraque>();
        List<TipoOperacionPortuaria> listaoperaciones = new List<TipoOperacionPortuaria>();

        //Variables
        protected bool IsDisabled = false;
        protected bool practicaje = false;
        protected bool embarque = false;
        protected bool desembarque = false;
        protected bool electricidad = false;
        bool renderizado = false;

        private DocPortuariaSoapClient miservicio = new DocPortuariaSoapClient(DocPortuariaSoapClient.EndpointConfiguration.DocPortuariaSoap);

        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (EditarNuevo == 1) { IsDisabled = true; }
                await ActualizaDatos();
               
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EditarAtraque.OnInitializedAsync: {ex.Message}");
            }
        }

        private async Task ActualizaDatos()
        {
            try

            {
                if (EditarNuevo!=0)
                {
                    atraque = await DataContext.Atraques
                              .FirstOrDefaultAsync(p => p.ID == id && p.N_Atraque == N_Atraque);


                    if (atraque.Praticaje == 1) { practicaje = true; }
                    if (atraque.OperacionE == 1) { embarque = true; }
                    if (atraque.OperacionD == 1) { desembarque = true; }
                    if (atraque.Electricidad == 1) { electricidad = true; }


                }

               
                escala = await DataContext.Escalas
                            .Include(x => x.Barco)
                            .Include(x => x.PuertoOperacion)
                           .FirstOrDefaultAsync(x => x.ID_Atraque == id);

                //barco = await DataContext.Barcos
                //            .FirstOrDefaultAsync(x => x.Codigo == escala.Buque);

                //puerto = await DataContext.Puertos
                //            .FirstOrDefaultAsync(x => x.Codigo == escala.Puerto);

                listamuelles = await DataContext.Muelles
                              .Where(p => p.SUBPUERTO == escala.Puerto)
                              .OrderBy(p => p.MUELLE_DESC)
                              .ToListAsync();

                listatiposatraques = await DataContext.TiposAtraque
                             .OrderBy(p => p.Descripcion)
                             .ToListAsync();

                listaoperaciones = await DataContext.TiposOperaciones
                           .OrderBy(p => p.Descripcion)
                           .ToListAsync();


                StateHasChanged();


            }
            catch (Exception ex)
            {
                Console.WriteLine($"EditarAtraque.ActualizaDatos: {ex.Message}");

            }
        }
        //Guardar
        protected async Task Guardar()
        {

            renderizado = true;
            atraque.Praticaje = 0;
            atraque.OperacionE = 0;
            atraque.OperacionD = 0;
            atraque.Electricidad = 0;
            if (practicaje) { atraque.Praticaje = 1; }
            if (embarque) { atraque.OperacionE = 1; }
            if (desembarque) { atraque.OperacionD = 1; }
            if (electricidad) { atraque.Electricidad = 1; }
            if (embarque == false && desembarque == false) { atraque.TipoActividad = "ZOT"; }

            if (atraque.Ocupacíon < atraque.Desocupacíon)

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
            else
            {
                ToastService.ShowError("La fecha-hora de desocupación no puede ser superior a la fecha-hora de ocupación.", "Error");
            }

          
            renderizado = false;
        }

        //Actualiza el registro seleccionado
        protected async Task EditarRegistro()
        {
            try
            {
                if (escala.NumeroEscala < 100000)
                {
                    atraque.Estado = "MTR";
                    await DataContext.SaveChangesAsync();
                    StateHasChanged();


                    if (puerto.Autonomico == 0)

                    {

                        //ToastService.ShowWarning("Enviando modificación a Puertos del Estado.", "Espere...");
                        
                        //Reemplazo de atraque      
                        await miservicio.NuevoAltaBermanAsync(escala.NumeroEscala, escala.AnnoEscala, escala.Puerto, true, 21,
                                atraque.N_Atraque, atraque.TipoAtraque, Convert.ToDateTime(atraque.Ocupacíon), Convert.ToDateTime(atraque.Desocupacíon));

                                               
                        //Modificar ETA en la escala
                        Atraque atraques_escala = new Atraque();
                        atraques_escala = await DataContext.Atraques
                              .OrderBy(p =>p.N_Atraque)
                              .FirstOrDefaultAsync(p => p.ID == id);

                        escala.FechaAtraque= System.Convert.ToDateTime(String.Format("{0:d}", atraques_escala.Ocupacíon));
                        escala.HoraLLegada = new DateTime(1899, 12, 30, atraques_escala.Ocupacíon.Value.Hour, atraques_escala.Ocupacíon.Value.Minute, 0);
                        //escala.HoraLLegada = System.Convert.ToDateTime(String.Format("{0:t}", atraques_escala.Ocupacíon));

                        //Modificar ETD en la escala
                        atraques_escala = await DataContext.Atraques
                              .OrderBy(p => p.N_Atraque)
                              .LastOrDefaultAsync(p => p.ID == id);

                        escala.FechaDesatraque = System.Convert.ToDateTime(String.Format("{0:d}", atraques_escala.Desocupacíon));
                        escala.HoraDesatraque = new DateTime(1899, 12, 30, atraques_escala.Desocupacíon.Value.Hour, atraques_escala.Desocupacíon.Value.Minute, 0);
                        //escala.HoraDesatraque = System.Convert.ToDateTime(String.Format("{0:t}", atraques_escala.Desocupacíon));

                        //Modificar el muelle de atraque y practicaje
                        escala.Muelle = atraques_escala.Muelle_ID;
                        escala.Practicaje = atraque.Praticaje;

                        await DataContext.SaveChangesAsync();
                        StateHasChanged();

                        //ToastService.ShowWarning("Enviando modificación a Puertos del Estado.", "Espere...");
                        await miservicio.AltaBermanAsync(escala.NumeroEscala, escala.AnnoEscala, escala.Puerto, true, 54, Convert.ToInt32(escala.Practicaje),0);

                        ToastService.ShowSuccess("Enviada la modificación de la escala.", "Correcto");

                    }
                    else
                    {
                        ToastService.ShowSuccess("El registro se actualizó correctamente.", "Correcto");
                    }
                }
                else
                {
                    ToastService.ShowError("No se pueden crear nuevos atraques hasta que la escala se autorice.Por favor, inténtelo más tarde.", "Error");
                }
                
                AccederPaginaInicial();

            }

            catch (Exception ex)
            {
                Console.WriteLine($"EditarAtraque.EditarRegistro: {ex.Message}");
                ToastService.ShowError("Los servidores no se encuentran disponibles. Por favor, inténtelo de nuevo más tarde.", "Error");
            }
        }

        // Inserta un nuevo registro
        protected async Task AgregarRegistro()
        {
            try
            {

               if (escala.NumeroEscala < 100000)
                {
                    atraque.ID = id;
                    atraque.N_Atraque = Convert.ToInt32(escala.N_Atraques) + 1;
                    atraque.Estado = "ATR";
                    escala.N_Atraques = atraque.N_Atraque;
                    escala.Practicaje = atraque.Praticaje;
                    escala.FechaDesatraque = System.Convert.ToDateTime(String.Format("{0:d}", atraque.Desocupacíon));
                    //escala.HoraDesatraque = System.Convert.ToDateTime(String.Format("{0:t}", atraque.Desocupacíon));
                    escala.HoraDesatraque = new DateTime(1899, 12, 30, atraque.Desocupacíon.Value.Hour, atraque.Desocupacíon.Value.Minute, 0);
                    escala.Muelle = atraque.Muelle_ID;
                   
                    await DataContext.Atraques.AddAsync(atraque);
                    await DataContext.SaveChangesAsync();
                    await ActualizaDatos();
                                        
                    AccederPaginaInicial();

                    if (puerto.Autonomico == 0)
                    {
                        
                        //ToastService.ShowWarning("Enviando la solicitud de escala a Puertos del Estado.", "Espere...");
                                                
                        //Alta de atraque
                        await miservicio.NuevoAltaBermanAsync(escala.NumeroEscala, escala.AnnoEscala, escala.Puerto, true, 13,
                                atraque.N_Atraque, atraque.TipoAtraque, Convert.ToDateTime(atraque.Ocupacíon), Convert.ToDateTime(atraque.Desocupacíon));

                        //Modificación de ETA
                        await miservicio.AltaBermanAsync(escala.NumeroEscala, escala.AnnoEscala, escala.Puerto, true, 54, Convert.ToInt32(escala.Practicaje),0);

                        ToastService.ShowSuccess("Solicitud de escala enviada.", "Correcto");
                    }
                    else
                    {
                        ToastService.ShowSuccess("El registro se almacenó correctamente.", "Correcto");
                    }

                }
                else
                {
                    ToastService.ShowError("No se pueden crear nuevos atraques hasta que la escala se autorice.Por favor, inténtelo más tarde.", "Error");
                }
                             

            }

            catch (Exception ex)
            {
                Console.WriteLine($"EditarAtraque.AgregarRegistro: {ex.Message}");
                ToastService.ShowError("Los servidores no se encuentran disponibles. Por favor, inténtelo de nuevo más tarde.", "Error");
            }
        }
        protected void AccederPaginaInicial()
        {
            try
            {
                UriHelper.NavigateTo($"/Atraques/"+ id + "/" + escala.NumeroEscala + "/" + escala.AnnoEscala + "/" + 
                    escala.Puerto + "/" + escala.Buque);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EditarAtraque.AccederPaginaInicial: {ex.Message}");
            }
        }

        protected async Task Operacion(ChangeEventArgs e)
        {
            try
            {
                if (e.Value.ToString() == "ZOT")
                {
                    embarque = false;
                    desembarque = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EditarAtraque.Operacion: {ex.Message}");
            }
        }

    }
}
