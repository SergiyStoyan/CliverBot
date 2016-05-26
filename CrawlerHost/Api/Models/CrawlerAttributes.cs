using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.Entity;

namespace Cliver.CrawlerHost.Models
{
    [MetadataType(typeof(CrawlerAttributes))]
    public partial class Crawler
    {
    }

    sealed class CrawlerAttributes
    {
        [Key]
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Crawler")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "State")]
        public int State { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Site")]
        public string Site { get; set; }

        [Required]
        [Display(Name = "Command")]
        public int Command { get; set; }
        
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Run Time Span")]
        public int RunTimeSpan { get; set; }
        
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Crawl Product Timeout (secs)")]
        public int CrawlProductTimeout { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Yield Product Timeout (secs)")]
        public int YieldProductTimeout { get; set; }
        
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Admin Emails (by new line)")]
        public string AdminEmails { get; set; }

        //[Required(AllowEmptyStrings = true)]
        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Restart Delay If Broken (secs)")]
        public int RestartDelayIfBroken { get; set; }

        [Display(Name = "_SessionStartTime")]
        public DateTime? _SessionStartTime { get; set; }

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

        [Display(Name = "_ProductsTable")]
        public string _ProductsTable { get; set; }

        [Display(Name = "_LastProductTime")]
        public DateTime? _LastProductTime { get; set; } 
    }
}