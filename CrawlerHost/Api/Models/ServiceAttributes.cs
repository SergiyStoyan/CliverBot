using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.Entity;

namespace Cliver.CrawlerHost.Models
{
    [MetadataType(typeof(ServiceAttributes))]
    public partial class Service
    {
    }

    sealed class ServiceAttributes
    {
        [Key]
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Service")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "State")]
        public int State { get; set; }

        [Required]
        [Display(Name = "Command")]
        public int Command { get; set; }
        
        [Required]
        [Display(Name = "Run Time Span")]
        public int RunTimeSpan { get; set; }
        
        [Required]
        [Display(Name = "Life Cycle Timeout (secs)")]
        public int RunTimeout { get; set; }
        
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Admin Emails (by new line)")]
        public string AdminEmails { get; set; }

        //[Required(AllowEmptyStrings = true)]
        [Display(Name = "Comment")]
        public string Comment { get; set; }

        //[Required(AllowEmptyStrings = true)]
        [Display(Name = "Folder With Service Exe File")]
        public string ExeFolder { get; set; }

        [Display(Name = "_LastSessionState")]
        public DateTime? _LastSessionState { get; set; }
        
        [Display(Name = "_NextStartTime")]
        public DateTime? _NextStartTime { get; set; }
        
        [Display(Name = "_LastStartTime")]
        public DateTime? _LastStartTime { get; set; }

        [Display(Name = "_LastEndTime")]
        public DateTime? _LastEndTime { get; set; }

        [Display(Name = "_LastProcessId")]
        public int? _LastProcessId { get; set; }

        [Display(Name = "_LastLog")]
        public string _LastLog { get; set; }

        [Display(Name = "_Archive")]
        public string _Archive { get; set; }
    }
}