using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.ErrorLogArea.Models
{
    public class LogXmlPathModel
    {
        public string moduleId { get; set; }
        public string LogKey { get; set; }
        public string LogPath { get; set; }
        public string FileAlias { get; set; }
    }
}