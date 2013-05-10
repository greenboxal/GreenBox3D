using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*using Awesomium.Core.Data;
using GreenBox3D.Content;

namespace GreenBox3D.Awesomium
{
    public class EngineDataSource : DataSource
    {
        protected unsafe override void OnRequest(DataSourceRequest request)
        {
            Stream s = FileManager.OpenFile(Path.Combine("WebUI", request.Path.Split('?')[0]));

            if (s == null)
            {
                SendRequestFailed(request);
                return;
            }

            byte[] data = new byte[s.Length];
            s.Read(data, 0, data.Length);
            s.Close();

            fixed (byte* ptr = &data[0])
                SendResponse(request, new DataSourceResponse
                {
                    Buffer = (IntPtr)ptr,
                    MimeType = request.MimeType,
                    Size = (uint)data.Length
                });
        }
    }
}
*/