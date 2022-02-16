using Syncfusion.Dashboard.Service.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Syncfusion.Dashboard.Designer.Web.Service
{
    public class DesignHelper : IDashboardDesignerController
    {

        [CryptoAttribute(CryptoMode.Encrypt)]
        public string EncryptData(string plaintext)
        {
            var keyCode = DashboardDesignerHelper.GetEncodedCode();
            if (!string.IsNullOrEmpty(keyCode))
            {
              //  return AESEncrytDecry.EncryptStringAES(plaintext, keyCode);
            }
            return plaintext;
        }

        [CryptoAttribute(CryptoMode.Decrypt)]
        public string DecryptData(string chiperText)
        {
            var keyCode = DashboardDesignerHelper.GetEncodedCode();
            if (!string.IsNullOrEmpty(keyCode))
            {
                //return AESEncrytDecry.DecryptStringAES(chiperText, keyCode);
            }
            return chiperText;
        }

        public Task<object> PostDesignerAction(Dictionary<string, object> jsonResult)
        {
            throw new NotImplementedException();
        }

        public void UploadReportAction()
        {
            throw new NotImplementedException();
        }

        public object GetImage(string key, string image)
        {
            throw new NotImplementedException();
        }

        //public bool UploadFile(HttpPostedFile httpPostedFile)
        //{
        //    throw new NotImplementedException();
        //}

        public FileModel GetFile(string filename, bool isOverride)
        {
            throw new NotImplementedException();
        }

        public List<FileModel> GetFiles(FileType fileType)
        {
            throw new NotImplementedException();
        }

        public string GetFilePath(string key)
        {
            throw new NotImplementedException();
        }
    }
}