using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.Entity;

namespace Cliver.CrawlerHost.Models
{
    [MetadataType(typeof(MessageAttributes))]
    public partial class Message
    {
    }

    sealed class MessageAttributes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Source")]
        public string Source { get; set; }

        [Required]
        [Display(Name = "Type")]
        public int Type { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Message")]
        public string Value { get; set; }

        [Required]
        [Display(Name = "Time")]
        public DateTime Time { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Details")]
        public string Details { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Mark")]
        public int Mark { get; set; }
    }
}