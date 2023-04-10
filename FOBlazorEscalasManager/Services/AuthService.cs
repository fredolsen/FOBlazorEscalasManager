using Blazored.LocalStorage;
using FOBlazorEscalasManager.Helpers;
using FOBlazorEscalasManager.Models;
using FOBlazorEscalasManager.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FOBlazorEscalasManager.Services
{
    public class AuthService
    {

        private readonly byte[] key;
        private DataContext context;
        private readonly Estado estado;
        private readonly ILocalStorageService localStorage;
        private readonly NavigationManager navigationManager;
       

        public AuthService(DataContext context,
            IConfiguration configuration,
            Estado estado,
            ILocalStorageService localStorage,
            NavigationManager navigationManager)

        {
            this.context = context;
            this.estado = estado;
            this.localStorage = localStorage;
            this.navigationManager = navigationManager;
            key = Encoding.ASCII.GetBytes(configuration.GetSection("AppSettings:secret").Value);
        }

        public async Task CerrarSesion()
        {
            estado.Token = null;
            await localStorage.RemoveItemAsync("authModel");
        }

        public async Task<bool> Autorizar()
        {
            var authModel = await localStorage.GetItemAsync<AuthModel>("authModel");

            var estaAutorizado = false;
            estado.Token = null;

            if (authModel != null)
            {
                var validationParameters = new TokenValidationParameters()
                {
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken validatedToken;
                try
                {
                    tokenHandler.ValidateToken(authModel.Token, validationParameters, out validatedToken);
                }
                catch
                {
                    return estaAutorizado;
                }

                if (validatedToken != null)
                {
                    estado.Token = authModel;
                    estaAutorizado = true;
                }
            }

            return estaAutorizado;
        }

        public async Task<string> Login(string username, string password)
        {
            try
            {
                AuthModel authModel;

                var mensaje = "";
                estado.Token = null;

                username = username.Trim();
                var usuariolocal = await context.UsersProfile.FirstOrDefaultAsync(p => p.UserName == username);
                if (usuariolocal != null)
                {
                    var validado = ActiveDirectoryHelper.Login(username, password);
                    if (!validado)
                    {
                        return "El nombre de usuario o contraseña no són válidos";
                    }

                    var user = new Usuario
                    {
                        Username = username,
                        Rol = RolEnum.Administrador
                    };

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                    new Claim(ClaimTypes.NameIdentifier, username.ToLower()),
                    new Claim(ClaimTypes.Name, username.ToLower()),
                        }),
                        Expires = DateTime.Now.AddDays(10),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                            SecurityAlgorithms.HmacSha512Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var tokenString = tokenHandler.WriteToken(token);

                    authModel = new AuthModel { Token = tokenString, Usuario = user, Mensaje = mensaje };
                    estado.Token = authModel;

                    await localStorage.SetItemAsync("authModel", authModel);
                    return string.Empty;
                }
                else
                {
                    return "Este usuario no tiene permisos para acceder a la aplicación.";
                }

            }
            catch
            {
                return "Se ha producido un error inesperado. Si persiste, contácte con el departamento de RRHH para resolver esta situación.";
            }
        }
    }
}
