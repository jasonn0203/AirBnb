//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AirBnb.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class HinhAnh
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HinhAnh()
        {
            this.Phongs = new HashSet<Phong>();
        }
    
        public int MaHinhAnh { get; set; }
        public string HinhAnh1 { get; set; }
        public string HinhAnh2 { get; set; }
        public string HinhAnh3 { get; set; }
        public string HinhAnh4 { get; set; }
        public string HinhAnh5 { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Phong> Phongs { get; set; }
    }
}
