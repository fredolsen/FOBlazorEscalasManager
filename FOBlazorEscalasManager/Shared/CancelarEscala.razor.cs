using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;

namespace FOBlazorEscalasManager.Shared
{
    public partial class CancelarEscala : ComponentBase
    {

        [CascadingParameter] ModalParameters Parameters { get; set; }
        [CascadingParameter] BlazoredModal BlazoredModal { get; set; }

        string Comentario { get; set; } = "¿Está seguro que desea cancelar esta escala?";

        [Inject]
        IModalService ModalService { get; set; }

        protected bool afirmativo = false;

        protected override void OnInitialized()
        {
            try
            {
                if (Parameters.Get<string>("Dialogo") != "")
                {
                    Comentario = Parameters.Get<string>("Dialogo");
                }
            }
            catch (Exception ex)
            {

            }                 

        }
        protected async Task Guardar(bool _afirmativo)
        {
            ModalService.Close(ModalResult.Ok<bool>(_afirmativo));
        }

    }
}
