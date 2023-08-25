using System;
using System.Collections.Generic;

namespace ICE.GDocs.Domain.ExternalServices
{
    public interface IRdlToPdfBytesConverterExternalService
    {
        TryException<byte[]> Converter(string rdlPath, Dictionary<string, string> queryStringParameters = null);
    }
}
