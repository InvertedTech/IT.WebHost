using IT.WebServices.Fragments.Careers;

namespace Admin.Models
{
    public class CareerEditModel
    {
        public string CareerId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Contact { get; set; } = string.Empty;
        public string BodyMarkdown { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public bool RelocationRequired { get; set; }
        public JobType EmploymentType { get; set; }
        public DateTime? CreatedOnUTC { get; set; }
        public DateTime? ModifiedOnUTC { get; set; }
        public DateTime? DeletedOnUTC { get; set; }

        public static CareerEditModel FromRecord(CareerRecord record) => new()
        {
            CareerId = record.CareerId,
            Title = record.Title,
            Company = record.Company,
            Department = record.Department,
            Contact = record.Contact,
            BodyMarkdown = record.BodyMarkdown,
            Area = record.Location?.Area ?? string.Empty,
            RelocationRequired = record.Location?.RelocationRequired ?? false,
            EmploymentType = record.Location?.EmploymentType ?? JobType.FullTime,
            CreatedOnUTC = record.CreatedOnUTC?.ToDateTime(),
            ModifiedOnUTC = record.ModifiedOnUTC?.ToDateTime(),
            DeletedOnUTC = record.DeletedOnUTC?.ToDateTime(),
        };

        public CareerRecord ToRecord() => new()
        {
            CareerId = CareerId,
            Title = Title,
            Company = Company,
            Department = Department,
            Contact = Contact,
            BodyMarkdown = BodyMarkdown,
            Location = new ListingLocation
            {
                Area = Area,
                RelocationRequired = RelocationRequired,
                EmploymentType = EmploymentType,
            },
        };
    }
}
