namespace project_comp1640_be.Model.Dto
{
    public class ContributionDTO
    {
        public int contribution_id { get; set; }
        public int user_id { get; set; }
        public string username { get; set; }
        public string contribution_image { get; set; }
        public string contribution_title { get; set; }
        public DateTime contribution_submition_date { get; set; }
        public DateTime final_clouser_date { get; set; }
        public string isView { get; set; }
        public string isSelected { get; set; }

        public string isEnabled { get; set; }
        public string isPublic { get; set; }

    }
}
