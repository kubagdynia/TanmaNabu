using System.Collections.Generic;

namespace Entitas
{
    public class AttributeInfo
    {
        public readonly object Attribute;
        public readonly List<PublicMemberInfo> MemberInfos;

        public AttributeInfo(object attribute, List<PublicMemberInfo> memberInfos)
        {
            Attribute = attribute;
            MemberInfos = memberInfos;
        }
    }
}
