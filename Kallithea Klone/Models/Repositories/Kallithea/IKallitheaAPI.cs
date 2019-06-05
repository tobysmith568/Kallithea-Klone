using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace KallitheaKlone.Models.Repositories.Kallithea
{
    public interface IKallitheaAPI
    {
        //  Methods
        //  =======

        Task<ICollection<Repository>> GetRepositories();
        
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="WebException"></exception>
        Task<bool> ValidateUserAccount(string host, string apiKey);
    }
}
