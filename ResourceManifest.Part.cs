namespace Bars.Gkh.Ris
 {
     using B4;
     using Bars.B4.Modules.ExtJs;

     using Bars.Gkh.Ris.Enums;

     public partial class ResourceManifest
     {
         protected override void AdditionalInit(IResourceManifestContainer container)
         {
            container.RegisterExtJsEnum<ObjectValidateState>();
        }
     }
 }