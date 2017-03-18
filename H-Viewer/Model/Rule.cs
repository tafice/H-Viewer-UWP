using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HViewer.Model
{
    public class Rule
    {
        public Selector item, idCode, title, uploader, cover, category, datetime, rating, tags, description;

        public Selector pictureId, pictureUrl, pictureThumbnail, pictureHighRes;
        public Selector commentItem, commentAvatar, commentAuthor, commentDatetime, commentContent;

        public PictureRule pictureRule;
        public VideoRule videoRule;
        public TagRule tagRule;
        public CommentRule commentRule;

        public Rule()
        {
        }
    }
}
