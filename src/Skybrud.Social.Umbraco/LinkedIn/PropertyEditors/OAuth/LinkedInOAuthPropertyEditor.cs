using ClientDependency.Core;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.PropertyEditors;

namespace Skybrud.Social.Umbraco.LinkedIn.PropertyEditors.OAuth {

    [PropertyEditor("Skybrud.Social.LinkedIn.OAuth", "Skybrud.Social - LinkedIn OAuth", "/App_Plugins/Skybrud.Social/LinkedIn/OAuth/PropertyEditor.html", ValueType = "JSON")]
    [PropertyEditorAsset(ClientDependencyType.Javascript, "/App_Plugins/Skybrud.Social/LinkedIn/OAuth/Controllers.js")]
    public class LinkedInOAuthPropertyEditor : PropertyEditor {
        
        protected override PreValueEditor CreatePreValueEditor() {
            return new LinkedInOAuthPreValueEditor();
        }

        [PropertyEditorAsset(ClientDependencyType.Javascript, "/App_Plugins/Skybrud.Social/LinkedIn/OAuth/Controllers.js")]
        internal class LinkedInOAuthPreValueEditor : PreValueEditor {

            [PreValueField("config", "Configuration", "/App_Plugins/Skybrud.Social/LinkedIn/OAuth/PreValueEditor.html")]
			public string Config { get; set; }
        
        }
    
    }

}