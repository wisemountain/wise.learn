using System;

namespace LearnNet
{

    public class ContractException : Exception
    {
        public ContractException(string msg)
            : base(msg)
        {
        }
    }

    public class Contract
    {
        public static void Assert(Boolean v)
        {
#if DEBUG
            if (!v)
            {
                throw new ContractException($"Assert violation");
            }
#endif // DEBUG
        }

        public static void Assert(Boolean v, string m)
        {
#if DEBUG
            if (!v)
            {
                throw new ContractException($"Assert violation. msg: {m}");
            }
#endif // DEBUG
        }
    }
}
