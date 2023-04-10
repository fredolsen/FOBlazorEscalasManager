
namespace FOBlazorEscalasManager.Models
{
    public class AuthModel
    {
        public AuthModel()
        {
            Usuario = new Usuario();
        }

        public string Token { get; set; }
        public string Mensaje { get; set; }
        public Usuario Usuario { get; set; }
    }
}
