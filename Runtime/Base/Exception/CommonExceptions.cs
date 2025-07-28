using System;

namespace CnoomFrameWork.Base
{
    /// <summary>
    /// 资源异常
    /// </summary>
    public class ResourceException : BaseFrameworkException
    {
        public ResourceException(int errorCode, string message) 
            : base(errorCode, message)
        {
        }
        
        public ResourceException(int errorCode, string message, Exception innerException) 
            : base(errorCode, message, innerException)
        {
        }
    }
    
    /// <summary>
    /// 网络异常
    /// </summary>
    public class NetworkException : BaseFrameworkException
    {
        public NetworkException(int errorCode, string message) 
            : base(errorCode, message)
        {
        }
        
        public NetworkException(int errorCode, string message, Exception innerException) 
            : base(errorCode, message, innerException)
        {
        }
    }
    
    /// <summary>
    /// UI异常
    /// </summary>
    public class UiException : BaseFrameworkException
    {
        public UiException(int errorCode, string message) 
            : base(errorCode, message)
        {
        }
        
        public UiException(int errorCode, string message, Exception innerException) 
            : base(errorCode, message, innerException)
        {
        }
    }
    
    /// <summary>
    /// 数据存储异常
    /// </summary>
    public class StorageException : BaseFrameworkException
    {
        public StorageException(int errorCode, string message) 
            : base(errorCode, message)
        {
        }
        
        public StorageException(int errorCode, string message, Exception innerException) 
            : base(errorCode, message, innerException)
        {
        }
    }
    
    /// <summary>
    /// 配置异常
    /// </summary>
    public class ConfigurationException : BaseFrameworkException
    {
        public ConfigurationException(int errorCode, string message) 
            : base(errorCode, message)
        {
        }
        
        public ConfigurationException(int errorCode, string message, Exception innerException) 
            : base(errorCode, message, innerException)
        {
        }
    }
    
    /// <summary>
    /// 依赖注入异常
    /// </summary>
    public class DependencyException : BaseFrameworkException
    {
        public DependencyException(int errorCode, string message) 
            : base(errorCode, message)
        {
        }
        
        public DependencyException(int errorCode, string message, Exception innerException) 
            : base(errorCode, message, innerException)
        {
        }
    }
    
    /// <summary>
    /// 事件系统异常
    /// </summary>
    public class EventException : BaseFrameworkException
    {
        public EventException(int errorCode, string message) 
            : base(errorCode, message)
        {
        }
        
        public EventException(int errorCode, string message, Exception innerException) 
            : base(errorCode, message, innerException)
        {
        }
    }
}