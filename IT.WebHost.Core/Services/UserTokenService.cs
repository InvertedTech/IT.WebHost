using System;
using System.Collections.Generic;
using System.Text;

namespace IT.WebHost.Core.Services
{
    public class UserTokenService
    {
        public string? Token { get; private set; }
        public void Initialize(string? token) => Token = token;
    }
}
