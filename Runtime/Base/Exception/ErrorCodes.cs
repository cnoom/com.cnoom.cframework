namespace CnoomFrameWork.Base
{
    /// <summary>
    /// 框架错误码定义
    /// </summary>
    public static class ErrorCodes
    {
        // 系统级错误 (1-999)
        public const int UnknownError = 1;
        public const int InvalidOperation = 2;
        public const int ArgumentError = 3;
        public const int NotImplemented = 4;
        public const int UnityInternalError = 5;
        public const int ConfigurationError = 6;
        public const int TimeoutError = 7;
        public const int SerializationError = 8;
        public const int DeserializationError = 9;
        
        // 资源管理错误 (1000-1999)
        public const int ResourceNotFound = 1000;
        public const int ResourceLoadFailed = 1001;
        public const int ResourceAlreadyLoaded = 1002;
        public const int ResourceReleaseError = 1003;
        public const int AddressableError = 1004;
        
        // 网络错误 (2000-2999)
        public const int NetworkError = 2000;
        public const int ConnectionFailed = 2001;
        public const int RequestTimeout = 2002;
        public const int InvalidResponse = 2003;
        public const int AuthenticationFailed = 2004;
        
        // UI错误 (3000-3999)
        public const int UiNotFound = 3000;
        public const int UiAlreadyOpen = 3001;
        public const int UiOperationFailed = 3002;
        
        // 数据存储错误 (4000-4999)
        public const int StorageError = 4000;
        public const int SaveFailed = 4001;
        public const int LoadFailed = 4002;
        public const int DataCorrupted = 4003;
        
        // 音频错误 (5000-5999)
        public const int AudioError = 5000;
        public const int AudioClipNotFound = 5001;
        public const int AudioPlayFailed = 5002;
        
        // 输入错误 (6000-6999)
        public const int InputError = 6000;
        public const int InputDeviceNotFound = 6001;
        
        // 依赖注入错误 (7000-7999)
        public const int DependencyError = 7000;
        public const int ServiceNotRegistered = 7001;
        public const int CircularDependency = 7002;
        public const int ResolutionFailed = 7003;
        
        // 事件系统错误 (8000-8999)
        public const int EventError = 8000;
        public const int EventHandlerError = 8001;
        
        // 池管理错误 (9000-9999)
        public const int PoolError = 9000;
        public const int PoolExhausted = 9001;
        public const int InvalidPoolObject = 9002;
    }
}