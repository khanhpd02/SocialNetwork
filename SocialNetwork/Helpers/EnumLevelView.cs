using System.ComponentModel;

namespace SocialNetwork.Helpers
{
    public enum EnumLevelView
    {
        [Description("Công khai")]
        publicview = 1
        ,
        [Description("Bạn bè")]
        friendview = 2
        ,
        [Description("Cá nhân")]
        privately = 3,
        [Description("Bạn thường")]
        friend = 4,
        [Description("Thân thiết")]
        bestfriend = 5
    }
}
