using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Session
{
    public class SessionContextAccessor : ISessionContextAccessor
    {
        public string TraceId => Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString();
    }

}
