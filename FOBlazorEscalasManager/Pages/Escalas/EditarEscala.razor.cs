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
using NodaTime;
using Microsoft.CodeAnalysis.Differencing;
using System.Runtime.InteropServices.WindowsRuntime;
using ServiceDocPortuaria;


namespace FOBlazorEscalasManager.Pages.Escalas
{
    public partial class EditarEscala : ComponentBase

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
        public int EditarNuevo { get; set; }
                
        //Clases
        protected Escala escala = new Escala();
        protected Puerto PuertoEscala = new Puerto();
             

        List<Puerto> listapuertos = new List<Puerto>();
        List<Barco> listabarcos = new List<Barco>();
        List<EstadoEscala> listaestados = new List<EstadoEscala>();
        List<Capitan> listacapitanes = new List<Capitan>();
        List<Muelle> listamuelles = new List<Muelle>();
        List<Atraque> listaatraques = new List<Atraque>();
        


        //Variables
        protected bool IsDisabled = false;
        protected bool practicaje=false;
        protected bool electricidad = false;
        protected bool IMDG_Carga= false;
        protected bool IMDG_Descarga=false;
        protected bool IMDG_Transito = false;
        protected string puertoselecionado=null;       
        bool renderizado = false;
        protected string puerto_desc;
        protected string barco_desc;
        DateTime Fecha_ETA = new DateTime();
        DateTime Hora_ETA = new DateTime();
        DateTime Fecha_ETD = new DateTime();
        DateTime Hora_ETD = new DateTime();

        private DocPortuariaSoapClient miservicio = new DocPortuariaSoapClient(DocPortuariaSoapClient.EndpointConfiguration.DocPortuariaSoap);



        protected override async Task OnInitializedAsync()
        {
            try
            {               
                if (EditarNuevo == 1) {IsDisabled = true;}
                await ActualizaDatos();               

            }
            catch (Exception ex)
            {
                Console.WriteLine($"EditarEscala.OnInitializedAsync: {ex.Message}");
            }
        }
        private async Task ActualizaDatos()
        {
            try
            {
                listapuertos = await DataContext.Puertos
                                .OrderBy(p => p.Descripcion)
                                .ToListAsync();

                listabarcos = await DataContext.Barcos
                                .OrderBy(p => p.Descripcion)
                                .ToListAsync();

                listaestados = await DataContext.EstadoEscalas
                              .ToListAsync();

                listacapitanes = await DataContext.Capitanes
                                .Where(p => p.Baja == 0)
                                .OrderBy(p => p.Nombre)
                                .ToListAsync();


                
                if (Escala != 0 || Año !=0 || Convert.ToInt16(Puerto) !=0)
                {
                    escala = await DataContext.Escalas
                                    .Include(x => x.Barco)
                                    .Include(x => x.PuertoOperacion)
                                    .FirstOrDefaultAsync(x => x.NumeroEscala == Escala
                                                        && x.AnnoEscala == Año
                                                        && (string.IsNullOrEmpty(Puerto) || x.Puerto == Puerto))
                                    ;

                    if (escala.Practicaje == 1) { practicaje = true; }
                    if (escala.IMDG_Carga == 1) { IMDG_Carga = true; }
                    if (escala.IMDG_Descarga == 1) { IMDG_Descarga = true; }
                    if (escala.IMDG_Transito == 1) { IMDG_Transito = true; }
                    


                    //barco_desc = listabarcos
                    //               .FirstOrDefault(x => x.Codigo == escala.Buque).Descripcion.ToString();

                    //puerto_desc = listapuertos
                    //              .FirstOrDefault(x => x.Codigo == escala.Puerto).Descripcion.ToString();

                    Fecha_ETA = Convert.ToDateTime(escala.FechaAtraque);
                    Hora_ETA = Convert.ToDateTime(escala.HoraLLegada);
                    Fecha_ETD = Convert.ToDateTime(escala.FechaDesatraque);
                    Hora_ETD = Convert.ToDateTime(escala.HoraDesatraque);
                }
                          
                listamuelles = await DataContext.Muelles
                              .Where(p => p.SUBPUERTO == escala.Puerto)
                              .OrderBy(p => p.MUELLE_DESC)
                              .ToListAsync();

                StateHasChanged();
               
            }

            catch (Exception ex)
            {
                Console.WriteLine($"EditarEscala.ActualizaDatos: {ex.Message}");
                ToastService.ShowError("Los servidores no se encuentran disponibles. Por favor, inténtelo de nuevo más tarde.", "Error");
            }
        }
        protected void AceptarFechaLLegada(DateTime fecha)
        {
            try
            {
                escala.FechaAtraque = fecha;
                //StateHasChanged();
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EditarEscala.AceptarFechaLLegada: {ex.Message}");
            }
        }
        protected void AceptarFechaSalida(DateTime fecha)
        {
            try
            {
                escala.FechaDesatraque = fecha;
                //StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EditarEscala.AceptarFechaSalida: {ex.Message}");
            }
        }
        protected void AceptarHoraLLegada(DateTime hora)
        {
            try
            {
                escala.HoraLLegada = hora;
               //StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EditarEscala.AceptarHoraLegada: {ex.Message}");
            }
        }
        protected void AceptarHoraSalida(DateTime hora)
        {
            try
            {
                escala.HoraDesatraque = hora;
                //StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EditarEscala.AceptarHoraSalida: {ex.Message}");
            }
        }
        protected async Task PuertoSeleccionado(ChangeEventArgs e)
        {

            try
            {
                escala.Muelle = "";
                escala.Puerto = e.Value.ToString();
                listamuelles = await DataContext.Muelles
                              .Where(p => p.SUBPUERTO == escala.Puerto)
                              .OrderBy(p => p.MUELLE_DESC)
                              .ToListAsync();
                //await ActualizaDatos();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"EditarEscala.PuertoSeleccionado: {ex.Message}");
            }

        }

        //Guardar
        protected async Task Guardar()
        {

            renderizado = true;
            escala.Practicaje = 0;
            escala.IMDG_Carga = 0;
            escala.IMDG_Descarga = 0;
            escala.IMDG_Transito = 0;
            if (practicaje) { escala.Practicaje = 1; }
            if (IMDG_Carga) { escala.IMDG_Carga = 1; }
            if (IMDG_Descarga) { escala.IMDG_Descarga = 1; }
            if (IMDG_Transito) { escala.IMDG_Transito = 1; }
           
          if ((Convert.ToDateTime(escala.FechaSalidaPtoAnterior).AddHours(Convert.ToDateTime(escala.HoraSalidaPtoAnterior).Hour).AddMinutes(Convert.ToDateTime(escala.HoraSalidaPtoAnterior).Minute)) > (Convert.ToDateTime(escala.FechaAtraque).AddHours(Convert.ToDateTime(escala.HoraLLegada).Hour).AddMinutes(Convert.ToDateTime(escala.HoraLLegada).Minute)))
            {
              
                ToastService.ShowError("La fecha-hora de salida del puerto anteriror no puede ser superior a la fecha-hora de llegada.", "Error");
            }
          else
            {
                if ((Convert.ToDateTime(escala.FechaAtraque).AddHours(Convert.ToDateTime(escala.HoraLLegada).Hour).AddMinutes(Convert.ToDateTime(escala.HoraLLegada).Minute)) > (Convert.ToDateTime(escala.FechaDesatraque).AddHours(Convert.ToDateTime(escala.HoraDesatraque).Hour).AddMinutes(Convert.ToDateTime(escala.HoraDesatraque).Minute)))
                {

                    ToastService.ShowError("La fecha-hora de llegada no puede ser superior a la fecha-hora de salida.", "Error");
                }
                else
                {
                    if ((Convert.ToDateTime(escala.FechaDesatraque).AddHours(Convert.ToDateTime(escala.HoraDesatraque).Hour).AddMinutes(Convert.ToDateTime(escala.HoraDesatraque).Minute)) > (Convert.ToDateTime(escala.FechaLLegadaPtoSiguiente).AddHours(Convert.ToDateTime(escala.HoraLLegadaPtoSiguiente).Hour).AddMinutes(Convert.ToDateTime(escala.HoraLLegadaPtoSiguiente).Minute)))
                    {
                        ToastService.ShowError("La fecha-hora de salida no puede ser superior a la fecha-hora de llegada al puerto siguiente.", "Error");
                    }
                    else
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
                   
                }
               
            }

            renderizado = false;
        }


        //Actualiza el registro seleccionado
        protected async Task EditarRegistro()
        {
            try
            {

                //var editarescala = await DataContext.Escalas
                //                    .FirstOrDefaultAsync(x => x.NumeroEscala == Escala && x.AnnoEscala == Año && x.Puerto == Puerto);

                
                if (escala.ID_Atraque != null)
                {
                    listaatraques = await DataContext.Atraques
                                    .Where(p => p.ID == escala.ID_Atraque)
                                    .OrderBy(p => p.N_Atraque)
                                    .ToListAsync();
                    if (listaatraques.Count > 0 )
                    {
                        listaatraques.FirstOrDefault().Ocupacíon = System.Convert.ToDateTime(String.Format("{0:d}", escala.FechaAtraque) + " " + String.Format("{0:t}", escala.HoraLLegada));
                        listaatraques.LastOrDefault().Desocupacíon = System.Convert.ToDateTime(String.Format("{0:d}", escala.FechaDesatraque) + " " + String.Format("{0:t}", escala.HoraDesatraque));
                    }
                    
                }
               
                //editarescala = escala;

                escala.HoraLLegada = new DateTime(1899, 12, 30, escala.HoraLLegada.Value.Hour, escala.HoraLLegada.Value.Minute, 0);
                escala.HoraDesatraque = new DateTime(1899, 12, 30, escala.HoraDesatraque.Value.Hour, escala.HoraDesatraque.Value.Minute, 0);
                escala.HoraSalidaPtoAnterior = new DateTime(1899, 12, 30, escala.HoraSalidaPtoAnterior.Value.Hour, escala.HoraSalidaPtoAnterior.Value.Minute, 0);
                escala.HoraLLegadaPtoSiguiente = new DateTime(1899, 12, 30, escala.HoraLLegadaPtoSiguiente.Value.Hour, escala.HoraLLegadaPtoSiguiente.Value.Minute, 0);
                escala.Estado = "ASM";
                //escala.HoraAtraque = escala.HoraLLegada;            

                await DataContext.SaveChangesAsync();
                StateHasChanged();
               
                //PuertoEscala = await DataContext.Puertos
                //                    .FirstOrDefaultAsync(x => x.Codigo == escala.Puerto);

                if (escala.PuertoOperacion.Autonomico == 0 && escala.NumeroEscala < 100000)

                {
                    //ToastService.ShowWarning("Enviando modificación a Puertos del Estado.", "Espere...");

                    //Modificación de la cabecera      
                        await miservicio.AltaBermanAsync(escala.NumeroEscala, escala.AnnoEscala, escala.Puerto, true, 33, Convert.ToInt32(escala.Practicaje), Convert.ToInt32(electricidad));                        
                   
                    if (escala.FechaAtraque != Fecha_ETA || escala.HoraLLegada != Hora_ETA || escala.FechaDesatraque != Fecha_ETD || escala.HoraDesatraque != Hora_ETD)
                    {
                        //Modificación de ETA
                        await miservicio.AltaBermanAsync(escala.NumeroEscala, escala.AnnoEscala, escala.Puerto, true, 54, Convert.ToInt32(escala.Practicaje), Convert.ToInt32(electricidad));
                    }                                        
                       
                    ToastService.ShowSuccess("Enviada la modificación de la escala.", "Correcto");

                }
                else
                {
                    ToastService.ShowSuccess("El registro se actualizó correctamente.", "Correcto");
                }

                AccederPaginaInicial();

            }

            catch (Exception ex)
            {
                Console.WriteLine($"EditarEscala.EditarRegistro: {ex.Message}");
                ToastService.ShowError("Los servidores no se encuentran disponibles. Por favor, inténtelo de nuevo más tarde.", "Error");
            }
        }

        // Inserta un nuevo registro
        protected async Task AgregarRegistro()
        {
            try
            {                
              
                await ActualizaDatos();

                escala.AnnoEscala = escala.FechaAtraque.Value.Year;
                escala.NumeroEscala = await Task.Run(() => GetUltimaEscala(escala.Puerto, escala.AnnoEscala));
                escala.Estado = "AAT";
                escala.Enviada = 0;
                //escala.FechaAtraque = escala.FechaAtraque.Date;
                //escala.FechaDesatraque = Convert.ToDateTime(escala.FechaDesatraque).Date;
                escala.HoraLLegada = new DateTime(1899, 12, 30, escala.HoraLLegada.Value.Hour, escala.HoraLLegada.Value.Minute, 0);
                escala.HoraDesatraque = new DateTime(1899, 12, 30, escala.HoraDesatraque.Value.Hour, escala.HoraDesatraque.Value.Minute, 0);
                escala.HoraSalidaPtoAnterior = new DateTime(1899, 12, 30, escala.HoraSalidaPtoAnterior.Value.Hour, escala.HoraSalidaPtoAnterior.Value.Minute, 0);
                escala.HoraLLegadaPtoSiguiente = new DateTime(1899, 12, 30, escala.HoraLLegadaPtoSiguiente.Value.Hour, escala.HoraLLegadaPtoSiguiente.Value.Minute, 0);
                //escala.HoraAtraque = escala.HoraLLegada;
                escala.N_Atraques = 0;

                
                await DataContext.Escalas.AddAsync(escala);
                await DataContext.SaveChangesAsync();           
                await ActualizaDatos();
                AccederPaginaInicial();

                if (escala.PuertoOperacion.Autonomico==0)
                {
                    escala.Estado = "ASO";
                    //ToastService.ShowWarning("Enviando la solicitud de escala a Puertos del Estado.", "Espere...");
                    //var result = await miservicio.CalculaTasaT1Async("S039", 3452, 2018);       

                    await miservicio.AltaBermanAsync(escala.NumeroEscala, escala.AnnoEscala, escala.Puerto, false, 47, Convert.ToInt32(escala.Practicaje), Convert.ToInt32(electricidad));
                    ToastService.ShowSuccess("Solicitud de escala enviada.", "Correcto");
                }
                else
                {
                    ToastService.ShowSuccess("El registro se almacenó correctamente.", "Correcto");
                }
                                                                               
              
            }

            catch (Exception ex)
            {
                Console.WriteLine($"EditarEscala.AgregarRegistro: {ex.Message}");
                ToastService.ShowError("Los servidores no se encuentran disponibles. Por favor, inténtelo de nuevo más tarde.", "Error");
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
                Console.WriteLine($"EditarEscala.AccederPaginaInicial: {ex.Message}");
            }
        }
        public async Task<int> GetUltimaEscala(string puerto, int año)
        {
            int Numero = 100000;          

            try
            {
                Escala Ultimaescala = new Escala();
                Ultimaescala = await DataContext.Escalas
                                    .OrderByDescending(x => x.NumeroEscala)
                                    .FirstOrDefaultAsync(x => x.Puerto == puerto && x.AnnoEscala == año);

                PuertoEscala = await DataContext.Puertos
                                    .FirstOrDefaultAsync(x => x.Codigo == puerto);
                if (Ultimaescala != null)
                {
                    if (PuertoEscala.Autonomico == -1 || PuertoEscala.Autonomico == 1 || Ultimaescala.NumeroEscala>=100000)
                    {
                        Numero = Ultimaescala.NumeroEscala + 1;
                    }                 

                }
                          
               
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EditarEscala.GetUltimaEscala: {ex.Message}");
            }
            finally
            {
                
            }
            return Numero;

        }
    }
}
