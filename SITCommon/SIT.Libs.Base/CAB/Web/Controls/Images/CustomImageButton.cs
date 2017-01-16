using System.ComponentModel;
using System.Drawing;
using System.Web.UI;
using System;
using SIT.Libs.Base.CAB.Web.Controls.Images;

[assembly: TagPrefix("SIT.Libs.Base.CAB.Web.Controls", "wc")]
[assembly: WebResource("SIT.Libs.Base.CAB.Web.Controls.Images.spinner.gif", "image/gif")]
[assembly: WebResource("SIT.Libs.Base.CAB.Web.Controls.Images.loading.gif", "image/gif")]
[assembly: WebResource("SIT.Libs.Base.CAB.Web.Controls.Images.error.gif", "image/gif")]
[assembly: WebResource("SIT.Libs.Base.CAB.Web.Controls.Images.warning.gif", "image/gif")]
[assembly: WebResource("SIT.Libs.Base.CAB.Web.Controls.Images.info.gif", "image/gif")]
[assembly: WebResource("SIT.Libs.Base.CAB.Web.Controls.Images.iconRefresh.gif", "image/gif")]
[assembly: WebResource("SIT.Libs.Base.CAB.Web.Controls.Images.iconHelp.gif", "image/gif")]
[assembly: WebResource("SIT.Libs.Base.CAB.Web.Controls.Images.iconInfo.gif", "image/gif")]
[assembly: WebResource("SIT.Libs.Base.CAB.Web.Controls.Images.siemens-logo.jpg", "image/gif")]
namespace SIT.Libs.Base.CAB.Web.Controls.Images
{
    /// <summary>
    /// A control wrapping System.Web.UI.WebControls.ImageButton used to display a custom 
    /// image compiled inside SIT.Libs.Base.CAB.dll
    /// </summary> 
    /// <remarks>This class is used since it is impossible to directly use WebResources
    /// inside a System.Web.UI.Page Object</remarks>
    [ToolboxBitmap(typeof(System.Web.UI.WebControls.ImageButton))]
    [Description("A control wrapping System.Web.UI.WebControls.ImageButton used to display a custom image compiled inside SIT.Libs.Base.CAB.dll")]
    internal class CustomImageButton : System.Web.UI.WebControls.ImageButton
    {
        /// <summary>
        /// Builds a new SIT.Libs.Base.CAB.Web.Controls.Images.CustomImage having
        /// AvailableImages.Loading as target image
        /// </summary>
        public CustomImageButton() : this(AvailableImages.Loading) { }

        /// <summary>
        /// Builds a new SIT.Libs.Base.CAB.Web.Controls.Images.CustomImage having a
        /// chosen targetimage
        /// </summary>
        /// <param name="TargetImage"></param>
        public CustomImageButton(AvailableImages TargetImage)
        {
            this.TargetImage = TargetImage;
        }

        /// <summary>
        /// The image to show
        /// </summary>
        public AvailableImages TargetImage
        {
            get
            {
                if (ViewState["TargetImage"] == null)
                    return AvailableImages.Loading;
                else
                    return (AvailableImages)ViewState["TargetImage"];
            }

            set
            {
                ViewState["TargetImage"] = value;
            }
        }

        /// <summary>
        /// Override
        /// </summary>
        public override string ImageUrl
        {
            get
            {
                switch (TargetImage)
                {
                    case AvailableImages.Loading:
                        return Page.ClientScript.GetWebResourceUrl(this.GetType(), "SIT.Libs.Base.CAB.Web.Controls.Images.spinner.gif");

                    case AvailableImages.Warning:
                        return Page.ClientScript.GetWebResourceUrl(this.GetType(), "SIT.Libs.Base.CAB.Web.Controls.Images.warning.gif");

                    case AvailableImages.Information:
                        return Page.ClientScript.GetWebResourceUrl(this.GetType(), "SIT.Libs.Base.CAB.Web.Controls.Images.info.gif");

                    case AvailableImages.Error:
                        return Page.ClientScript.GetWebResourceUrl(this.GetType(), "SIT.Libs.Base.CAB.Web.Controls.Images.error.gif");

                    case AvailableImages.IconRefresh:
                        return Page.ClientScript.GetWebResourceUrl(this.GetType(), "SIT.Libs.Base.CAB.Web.Controls.Images.iconRefresh.gif");

                    case AvailableImages.IconHelp:
                        return Page.ClientScript.GetWebResourceUrl(this.GetType(), "SIT.Libs.Base.CAB.Web.Controls.Images.iconHelp.gif");

                    case AvailableImages.IconInformation:
                        return Page.ClientScript.GetWebResourceUrl(this.GetType(), "SIT.Libs.Base.CAB.Web.Controls.Images.iconInfo.gif");

                    case AvailableImages.SiemensLogo:
                        return Page.ClientScript.GetWebResourceUrl(this.GetType(), "SIT.Libs.Base.CAB.Web.Controls.Images.siemens-logo.jpg");
                
                    default:
                        return Page.ClientScript.GetWebResourceUrl(this.GetType(), "SIT.Libs.Base.CAB.Web.Controls.Images.loading.gif");
                }
            }
            set
            {
                ;//do nothing
            }
        }
    }
}
