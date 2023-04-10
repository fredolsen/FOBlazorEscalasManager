using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace FOBlazorEscalasManager.Shared
{
    public partial class EliminarElemento : ComponentBase
    {
        [Inject]
        IModalService ModalService { get; set; }

        protected bool afirmativo = false;

        protected async Task Guardar(bool _afirmativo)
        {
            ModalService.Close(ModalResult.Ok<bool>(_afirmativo));
        }
    }
}
