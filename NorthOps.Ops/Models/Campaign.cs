//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NorthOps.Ops.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Campaign
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Campaign()
        {
            this.Users = new HashSet<User>();
        }
    
        public string CampaignId { get; set; }
        public string ClientId { get; set; }
        public string CampaignName { get; set; }
        public string CampaignDescription { get; set; }
        public Nullable<System.DateTime> DateTimePeriodFrom { get; set; }
        public Nullable<System.DateTime> DateTimePeriodTo { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
    
        public virtual Client Client { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User> Users { get; set; }
    }
}
