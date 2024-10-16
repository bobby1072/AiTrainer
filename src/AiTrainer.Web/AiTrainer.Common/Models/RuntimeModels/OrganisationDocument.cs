namespace AiTrainer.Web.Common.Models.RuntimeModels
{
    public class OrganisationDocument : RuntimeBase, IEquatable<OrganisationDocument>
    {
        public Guid? Id { get; set; }
        public string OriginalFileName { get; set; }
        public FileType OriginalFileType { get; set; }
        public Guid CompanyId { get; set; }
        public string DocumentText { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        public OrganisationDocument(
            string originalFileName,
            FileType originalFileType,
            Guid companyId,
            string documentText,
            DateTime dateCreated,
            DateTime dateModified,
            Guid? id = null
        )
        {
            Id = id;
            OriginalFileName = originalFileName;
            OriginalFileType = originalFileType;
            CompanyId = companyId;
            DocumentText = documentText;
            DateCreated = dateCreated;
            DateModified = dateModified;
        }

        public override bool Equals(RuntimeBase? other)
        {
            if (other == null)
            {
                return false;
            }
            else if (other is OrganisationDocument otherDocument)
            {
                return Id == otherDocument.Id
                    && OriginalFileName == otherDocument.OriginalFileName
                    && OriginalFileType == otherDocument.OriginalFileType
                    && CompanyId == otherDocument.CompanyId
                    && DocumentText == otherDocument.DocumentText
                    && DateCreated == otherDocument.DateCreated
                    && DateModified == otherDocument.DateModified;
            }
            return false;
        }

        public bool Equals(OrganisationDocument? other) => Equals(other);
    }
}
