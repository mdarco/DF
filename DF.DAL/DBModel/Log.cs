//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DF.DB.DBModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class Log
    {
        public int ID { get; set; }
        public System.DateTime LogDate { get; set; }
        public string LogIP { get; set; }
        public Nullable<int> UserID { get; set; }
        public string Username { get; set; }
        public string UserFullName { get; set; }
        public string Action { get; set; }
        public string Payload { get; set; }
        public string Note { get; set; }
    }
}
