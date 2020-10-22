using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System;

namespace website.Models {
    public class Submission {
        public int id { get; set; }
        public string fileName { get; set; }
        public string logoUrl { get; set; }
        public long size { get; set; }
        public string authorId { get; set; }
        public DateTime submissionDate { get; set; }
        public DateTime statusUpdated { get; set; }
        public string updatedById { get; set; }
        public string status { get; set; }
        public virtual ICollection<SubmissionNote> notes { get; set; }
        [ForeignKey("meta")]
        public int? metaId { get; set; }
        public virtual Meta meta { get; set; }
    }
}