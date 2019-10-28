using CS.BLL.SR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CS.WebUI.Models.SR
{
    public class FileModel: SR_FILES.Entity
    {
        public string SOURE { get; set; }
        public FileSource SOURE_OBJ { get; set; }
        public string FILE_PATH { get; set; }
    }

    public class FileSource
    {
        public string NAME { get; set; }
        public string USER_NAME { get; set; }
        public DateTime CREATE_TIME { get; set; }
    }

}