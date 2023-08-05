﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class AirbnbEntities : DbContext
    {
        public AirbnbEntities()
            : base("name=AirbnbEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ChuNha> ChuNhas { get; set; }
        public virtual DbSet<DanhGia> DanhGias { get; set; }
        public virtual DbSet<DanhMucPhong> DanhMucPhongs { get; set; }
        public virtual DbSet<DonDatPhong> DonDatPhongs { get; set; }
        public virtual DbSet<HoaDon> HoaDons { get; set; }
        public virtual DbSet<KhachThue> KhachThues { get; set; }
        public virtual DbSet<KhuyenMai> KhuyenMais { get; set; }
        public virtual DbSet<Phong> Phongs { get; set; }
        public virtual DbSet<YeuThich> YeuThiches { get; set; }
    
        public virtual int UpdateGia1Ngay()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("UpdateGia1Ngay");
        }
    
        public virtual int UpdateRentalDates()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("UpdateRentalDates");
        }
    }
}
