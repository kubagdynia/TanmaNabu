using System.Collections.Generic;

namespace Entitas;

public class AttributeInfo(object attribute, List<PublicMemberInfo> memberInfos)
{
    public readonly object Attribute = attribute;
    public readonly List<PublicMemberInfo> MemberInfos = memberInfos;
}