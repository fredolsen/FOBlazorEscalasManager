namespace FOBlazorEscalasManager.Models
{
    public enum RolEnum
    {
        Administrador,
        Usuario
    }


    public class Usuario
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public RolEnum Rol { get; set; }
    }

}