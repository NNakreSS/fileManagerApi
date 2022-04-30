using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fileManagerApi.ViewModel
{
    public class fileModel
    {
        public string fileId { get; set; }
        public string fileName { get; set; }
        public string fileUpploadTime { get; set; }
        public string fileType { get; set; }
        public string inFolderId { get; set; }
        public string fileUrl { get; set; }
        public string fileOwnerId { get; set; }
        public Nullable<int> fileDownloadCount { get; set; }
    }
}