using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fileManagerApi.ViewModel
{
    public class folderModel
    {
        public string folderId { get; set; }
        public string folderOwnerId { get; set; }
        public string folderName { get; set; }
        public string folderCreateTime { get; set; }
        public int folderFileCount { get; set; }
        public string folderGroupLevel { get; set; }
    }
}