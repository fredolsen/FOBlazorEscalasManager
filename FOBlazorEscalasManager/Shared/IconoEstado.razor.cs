using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;

namespace FOBlazorEscalasManager.Shared
{
    public partial class IconoEstado : ComponentBase
    {

        [Parameter]
        public string Estado { get; set; }
        [Parameter]
        public string Tamaño { get; set; } = "2x";


    }
}
