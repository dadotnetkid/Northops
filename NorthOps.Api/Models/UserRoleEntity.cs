using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NorthOps.Api.Models
{
    [MetadataTypeAttribute(typeof(UserRoleMetaData))]
    public partial class UserRole : IRole<string>
    {
    }
    public class UserRoleMetaData
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
    }
}