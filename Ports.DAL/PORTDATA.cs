//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ports.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class PORTDATA
    {
        public int ID { get; set; }
        public string NAME { get; set; }
        public string PortCode { get; set; }

        [MaxLength(3)]
        public string UnctadPortCode { get; set; }
        public string Country { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Url { get; set; }

        [MaxLength(5)]
        public string MainPortCode { get; set; }
    }
}
