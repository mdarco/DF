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
    
    public partial class DanceSelectionChoreographies
    {
        public int DanceSelectionID { get; set; }
        public int ChoreographyID { get; set; }
        public string Note { get; set; }
    
        public virtual Choreographies Choreographies { get; set; }
        public virtual DanceSelections DanceSelections { get; set; }
    }
}
