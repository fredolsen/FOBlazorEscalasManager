using Novell.Directory.Ldap;
using System;

namespace FOBlazorEscalasManager.Helpers
{
    public static class ActiveDirectoryHelper
    {
        // TODO: Pasar a settings
        private const string DOMAIN_NAME = "FOSA.local";
        private const string QUERY_USER = "user";
        private const string QUERY_PASSWORD = "Platon2014";

        public static bool Login(string username, string password)
        {
            var domainName = DOMAIN_NAME;
            string userDn = $"{username}@{domainName}";
            try
            {
                using (var connection = new LdapConnection { SecureSocketLayer = false })
                {
                    connection.Connect("172.16.1.12", LdapConnection.DefaultPort);
                    // connection.Connect(domainName, LdapConnection.DefaultPort);
                    connection.Bind(userDn, password);

                    if (connection.Bound)
                        return true;
                }
            }
            catch (LdapException ex)
            {
                // TOOD: Log exception
                Console.WriteLine(ex.Message);
            }
            return false;
        }


        public static string IdUsuario(string username)
        {
            LdapEntry usuarioLDAP = null;

            string userDn = $"{QUERY_USER}@{DOMAIN_NAME}";
            string IdEmpleado = null;
            try
            {
                using (var connection = new LdapConnection { SecureSocketLayer = false, Constraints = new LdapConstraints { ReferralFollowing = true } })
                {
                    connection.Connect(DOMAIN_NAME, LdapConnection.DefaultPort);
                    connection.Bind(userDn, QUERY_PASSWORD);

                    var result = connection.Search(
                        "DC=fosa,DC=local",
                        LdapConnection.ScopeSub,
                        "(&(objectCategory=person)(objectClass=user)(sAMAccountName=" + username.Trim().ToLower() + "))",
                        null,
                        false
                    );

                    if (result.HasMore())
                    {
                        var nextEntry = result.Next();
                        nextEntry.GetAttributeSet();
                        IdEmpleado = nextEntry.GetAttribute("employeeNumber").StringValue;
                        usuarioLDAP = nextEntry;
                    }
                }
            }
            catch (LdapException ex)
            {
                // TOOD: Log exception
                Console.WriteLine(ex.Message);
            }
            return IdEmpleado;
        }

    }
}
