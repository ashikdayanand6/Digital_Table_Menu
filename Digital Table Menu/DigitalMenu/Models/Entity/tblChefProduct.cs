//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DigitalMenu.Models.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class tblChefProduct
    {
        [Key]
        public int ChPdId { get; set; }
        [Required]
        [Display(Name ="Select Chef")]
        public Nullable<int> ChId { get; set; }
        [Required]
        public string Products { get; set; }
        [Required]
        public string PstDate { get; set; }
        [Required]
        public string Stataus { get; set; }
    
        public virtual tblChef tblChef { get; set; }
    }
}
