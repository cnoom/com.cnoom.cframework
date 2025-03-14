namespace CnoomFrameWork.Fsm
{
    public struct IFsmMessage<T>
    {
        public string MessageType;
        public T Value;

        public IFsmMessage(string messageType, T value)
        {
            MessageType = messageType;
            Value = value;
        }
    }
}