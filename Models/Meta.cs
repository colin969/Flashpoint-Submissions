using YamlDotNet.Serialization;

namespace website.Models {
    public class Meta {
        public int id { get; set; }
        public string Title { get; set; }
        [YamlMember(Alias="Alternate Titles")]
        public string AlternateTitles { get; set; }
        public string Library { get; set; }
        public string Series { get; set; }
        public string Developer { get; set; }
        public string Publisher { get; set; }
        [YamlMember(Alias="Play Mode")]
        public string PlayMode { get; set; }
        [YamlMember(Alias="Release Date")]
        public string ReleaseDate { get; set; }
        public string Version { get; set; }
        public string Languages { get; set; }
        public string Extreme { get; set; }
        public string Tags { get; set; }
        [YamlMember(Alias="Tag Categories")]
        public string TagCategories { get; set; }
        public string Source { get; set; }
        public string Platform { get; set; }
        public string Status { get; set; }
        [YamlMember(Alias="Application Path")]
        public string ApplicationPath { get; set; }
        [YamlMember(Alias="Launch Command")]
        public string LaunchCommand { get; set; }
        [YamlMember(Alias="Game Notes")]
        public string Notes { get; set; }
        [YamlMember(Alias="Original Description")]
        public string OriginalDescription { get; set; }
        [YamlMember(Alias="Curation Notes")]
        public string CurationNotes { get; set; }
        public virtual Submission submission { get; set; }
    }
}