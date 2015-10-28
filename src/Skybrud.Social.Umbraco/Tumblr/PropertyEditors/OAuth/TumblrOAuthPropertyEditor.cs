using ClientDependency.Core;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.PropertyEditors;

namespace Skybrud.Social.Umbraco.Tumblr.PropertyEditors.OAuth {

    [PropertyEditor("Skybrud.Social.Tumblr.OAuth", "Skybrud.Social - Tumblr OAuth", "/App_Plugins/Skybrud.Social/Tumblr/OAuth/PropertyEditor.html", ValueType = "JSON")]
    [PropertyEditorAsset(ClientDependencyType.Javascript, "/App_Plugins/Skybrud.Social/Tumblr/OAuth/Controllers.js")]
    public class TumblrOAuthPropertyEditor : PropertyEditor {
        
        protected override PreValueEditor CreatePreValueEditor() {
            return new TumblrOAuthPreValueEditor();
        }

        [PropertyEditorAsset(ClientDependencyType.Javascript, "/App_Plugins/Skybrud.Social/Tumblr/OAuth/Controllers.js")]
        internal class TumblrOAuthPreValueEditor : PreValueEditor {

            [PreValueField("config", "Configuration", "/App_Plugins/Skybrud.Social/Tumblr/OAuth/PreValueEditor.html")]
			public string Config { get; set; }
        
        }
    
    }

}