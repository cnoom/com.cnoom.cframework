using System;

namespace CnoomFrameWork.Base.Container
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class InjectAttribute : Attribute
    {
        public string ContainerName { get; set; } = "";

        public InjectAttribute(){}
        
        public InjectAttribute(string containerName = "")
        {
            ContainerName = containerName;
        }
    }
}