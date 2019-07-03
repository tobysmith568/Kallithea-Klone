﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;

namespace KallitheaKlone.Models.Kallithea
{
    public interface IKallitheaAPI
    {
        //  Methods
        //  =======

        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="WebException"></exception>
        Task<ICollection<Repository>> GetRepositories(string host, string apiKey);
        
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="WebException"></exception>
        Task<bool> ValidateUserAccount(string host, string apiKey);
    }
}
