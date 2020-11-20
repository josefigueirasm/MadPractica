using System;
using Es.Udc.DotNet.Amazonia.Model.DAOs.ClientDao;
using Es.Udc.DotNet.Amazonia.Model.ClientServiceImp.Exceptions;
using Ninject;
using Es.Udc.DotNet.ModelUtil.Transactions;
using Es.Udc.DotNet.ModelUtil.Exceptions;
using System.Collections.Generic;

namespace Es.Udc.DotNet.Amazonia.Model.ClientServiceImp
{
    public interface IClientService
    {

        [Inject]
        IClientDao ClientDao { set; }


        /// <summary>
        /// Registra un nuevo cliente.
        /// </summary>
        /// <param name="login"> Login único e identificativo. </param>
        /// <param name="clearPassword"> Contraseña en claro. </param>
        /// <param name="clientDetails"> Detalles de cliente. </param>
        /// <exception cref="DuplicateInstanceException"/>
        [Transactional]
        void RegisterClient(String login, String clearPassword, 
            ClientDetails clientDetails);


        /// <summary>
        /// Actualiza los datos de un cliente ya existente.
        /// </summary>
        /// <param name="login"> The user profile id. </param>
        /// <param name="clientDetails"> The user profile details. </param>
        /// <exception cref="InstanceNotFoundException"/>
        [Transactional]
        void UpdateUserProfileDetails(String login,
            ClientDetails clientDetails);

        /// <summary>
        /// Inicia sesión de un login determinado.
        /// </summary>
        /// <param name="login"> Name of the login. </param>
        /// <param name="password"> The password. </param>
        /// <param name="passwordIsEncrypted"> if set to <c> true </c> [password is encrypted]. </param>
        /// <returns> LoginResult </returns>
        /// <exception cref="InstanceNotFoundException"/>
        /// <exception cref="IncorrectPasswordException"/>
        [Transactional]
        LoginDetails Login(String login, String password,
            Boolean passwordIsEncrypted);

        /// <summary>
        /// Salida de sesión de un cliente autenticado.
        /// </summary>
        /// <exception cref="InstanceNotFoundException"/>
        [Transactional]
        void Logout(LoginDetails loginDetails);

        /// <summary>
        /// Define por defecto una tarjeta para un usuario.
        /// </summary>
        /// <exception cref="InstanceNotFoundException"/>
        [Transactional]
        void SetDefaultCard(String numberCard, String login);

        /// <summary>
        /// Lista tarjetas de un cliente por su login.
        /// </summary>
        /// <exception cref="InstanceNotFoundException"/>
        [Transactional]
        List<Card> ListCardsByClientLogin(string login);

    }
}