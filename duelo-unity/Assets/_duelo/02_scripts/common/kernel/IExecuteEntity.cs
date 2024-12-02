namespace Duelo.Common.Kernel
{
    public interface IExecuteEntity
    {
        public bool IsRunning { get; }
        public void Begin();
        public void End();
    }
}