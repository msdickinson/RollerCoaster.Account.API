using RollerCoaster.Account.API.UseCases.Exceptions.InvaildRequestsException;
using System;
using System.Collections.Generic;

namespace RollerCoaster.Account.API.UseCases.Exceptions
{
    public class InvaildRequestException : Exception
    {
        internal IList<InvaildRequestData> _invaildRequestDatas;
        public IList<InvaildRequestData> InvaildRequestDatas { get { return _invaildRequestDatas; } }
        public InvaildRequestException(IList<InvaildRequestData> invaildRequestDatas)
        {
            _invaildRequestDatas = invaildRequestDatas;
        }
    }
}
