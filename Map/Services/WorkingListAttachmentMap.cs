namespace Bars.Gkh.Ris.Map.Services
{
    using B4.Modules.Mapping.Mappers;
    using Entities.Services;

    public class WorkingListAttachmentMap : BaseEntityMap<WorkingListAttachment>
    {
        public WorkingListAttachmentMap() :
            base("Bars.Gkh.Ris.Entities.Services.WorkingListAttachment", "RIS_WORKLIST_ATTACHMENT")
        {
        }

        protected override void Map()
        {
            this.Reference(x => x.WorkList, "Перечень работ/услуг").Column("WORKLIST_ID").Fetch().NotNull();
            this.Reference(x => x.Attachment, "Файл-вложение").Column("ATTACHMENT_ID").Fetch();
        }
    }
}