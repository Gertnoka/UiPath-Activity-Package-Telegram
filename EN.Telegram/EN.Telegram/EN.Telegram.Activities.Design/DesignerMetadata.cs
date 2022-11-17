using System.Activities.Presentation.Metadata;
using System.ComponentModel;
using System.ComponentModel.Design;
using EN.Telegram.Activities.Design.Designers;
using EN.Telegram.Activities.Design.Properties;

namespace EN.Telegram.Activities.Design
{
    public class DesignerMetadata : IRegisterMetadata
    {
        public void Register()
        {
            var builder = new AttributeTableBuilder();
            builder.ValidateTable();

            var categoryAttribute = new CategoryAttribute($"{Resources.Category}");

            builder.AddCustomAttributes(typeof(TelegramScope), categoryAttribute);
            builder.AddCustomAttributes(typeof(TelegramScope), new DesignerAttribute(typeof(TelegramScopeDesigner)));
            builder.AddCustomAttributes(typeof(TelegramScope), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(SendMessage), categoryAttribute);
            builder.AddCustomAttributes(typeof(SendMessage), new DesignerAttribute(typeof(SendMessageDesigner)));
            builder.AddCustomAttributes(typeof(SendMessage), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(SendPhoto), categoryAttribute);
            builder.AddCustomAttributes(typeof(SendPhoto), new DesignerAttribute(typeof(SendPhotoDesigner)));
            builder.AddCustomAttributes(typeof(SendPhoto), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(SendFile), categoryAttribute);
            builder.AddCustomAttributes(typeof(SendFile), new DesignerAttribute(typeof(SendFileDesigner)));
            builder.AddCustomAttributes(typeof(SendFile), new HelpKeywordAttribute(""));


            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
}
