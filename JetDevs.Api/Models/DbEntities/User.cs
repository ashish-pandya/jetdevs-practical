using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JetDevs.Api.Models.DbEntities
{
    /// <summary>
    /// User
    /// Extendeds Properties from ASP.NET IdentityUser
    /// </summary>
    public class User : IdentityUser
    {
        /// <summary>
        /// First Name
        /// </summary>
        [StringLength(100)]
        public string FirstName { get; set; }
        /// <summary>
        /// Last Name
        /// </summary>
        [StringLength(100)]
        public string LastName { get; set; }
        /// <summary>
        /// Record creation date time
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Modified date time
        /// </summary>
        public DateTime ModifiedDate { get; set; }

    }
}
