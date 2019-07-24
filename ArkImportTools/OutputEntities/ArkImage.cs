using System;
using System.Collections.Generic;
using System.Text;

namespace ArkImportTools.OutputEntities
{
    public class ArkImage
    {
        public string image_url;
        public string image_thumb_url;

        public static readonly ArkImage MISSING_ICON = new ArkImage
        {
            image_thumb_url = "https://icon-assets.deltamap.net/legacy/broken_item_thumb.png",
            image_url = "https://icon-assets.deltamap.net/legacy/broken_item.png"
        };
    }
}
