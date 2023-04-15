namespace Softfluent.Asapp.Core.Context
{
    public class CallContext
    {
        public CallContext()
            : this(Guid.NewGuid())
        {
        }

        public CallContext(Guid traceId)
        {
            TraceId = traceId;
            ExecutionIdentity = "System";
        }

        public string ExecutionIdentity { get; set; }

        public Guid TraceId { get; set; }
    }
}
