//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PMGSY.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ADMIN_NEWS_FILES
    {
        public int NEWS_ID { get; set; }
        public int FILE_ID { get; set; }
        public string FILE_TYPE { get; set; }
        public string FILE_NAME { get; set; }
        public string FILE_DESC { get; set; }
        public System.DateTime FILE_UPLOAD_DATE { get; set; }
    
        public virtual ADMIN_NEWS ADMIN_NEWS { get; set; }
    }
}
