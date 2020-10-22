using System;

namespace website.Models {
    public class SubmissionNote {
        public int id { get; set; }
        public virtual Submission submission { get; set; }
        public string authorId { get; set; }
        public DateTime dateAdded { get; set; }
        public string note { get; set; }
    }

}