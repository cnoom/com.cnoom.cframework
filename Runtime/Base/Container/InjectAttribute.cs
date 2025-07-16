using System;

namespace CnoomFrameWork.Base.Container
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class InjectAttribute : Attribute
    {
        public InjectAttribute()
        {
        }

        public InjectAttribute(string containerName = "")
        {
            ContainerName = containerName;
        }

        public string ContainerName { get; set; } = "";
    }
}