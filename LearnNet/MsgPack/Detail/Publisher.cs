using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LearnNet
{
    public class Publisher
    {
        public enum Result
        {
            Success, 
            Fail_Existing_Subscription, 
            Fail_MsgType_Mismatch,
        }

        private class Sub
        {
            public object Owner { get; set; }

            public Action<Msg> Action { get; set; }

            public uint MsgType { get; set; }

            public bool Unsubscribed { get; set; }

            public Sub()
            {
                Unsubscribed = false;
            }
        }

        private class TypeSubs
        {
            private List<Sub> subs = new List<Sub>();
            private uint msgType = 0;

            public uint MsgType { get { return msgType;  } }

            private TickTimer purgeTimer = new TickTimer();

            public TypeSubs(uint msgType)
            {
                this.msgType = msgType;
            }

            public Result Subscribe(Sub sub)
            {
                if ( sub.MsgType != msgType )
                {
                    return Result.Fail_MsgType_Mismatch;
                }
                
                if ( IsDuplicate(sub) )
                {
                    return Result.Fail_Existing_Subscription;
                }

                subs.Add(sub);

                return Result.Success;
            }

            public int Post(Msg m)
            {
                int postCount = 0;

                if ( m.Type == msgType)
                {
                    var lst = new List<Action<Msg>>();

                    foreach ( var sub in subs )
                    {
                        if (sub.Unsubscribed == false)
                        {
                            sub.Action(m);
                            ++postCount;
                        }
                    }
                }

                // 최적화. 하나의 타잎에 대한 등록해제 삭제. 가끔씩 실행
                if (purgeTimer.Elapsed() > 5000)
                {
                    // unsubscribed 지움. 하나의 타잎에 대한 등록이라 작음. 
                    subs.RemoveAll((sub) => sub.Unsubscribed == true);
                }

                return postCount;
            }

            public int GetSubscriptionCount()
            {
                return subs.Count((x) => { return x.Unsubscribed == false; });
            }

            public int GetSubscriptionCount(object o)
            {
                return subs.Count((x) => { return x.Owner == o && x.Unsubscribed == false; });
            }

            public void Unsubscribe(object owner, uint msgType)
            {
                foreach ( var sub in subs )
                {
                    if ( sub.Owner == owner && msgType == sub.MsgType  )
                    {
                        sub.Unsubscribed = true;
                    }
                }
            }

            public void Unsubscribe(object owner)
            {
                foreach ( var sub in subs )
                {
                    if ( sub.Owner == owner)
                    {
                        sub.Unsubscribed = true;
                    }
                }
            }

            public bool IsDuplicate(Sub sub)
            {
                return subs.Find(x => x.Owner == sub.Owner && x.MsgType == sub.MsgType) != null;
            }
        }

        private ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();
        private Dictionary<uint, TypeSubs> subs = new Dictionary<uint, TypeSubs>();

        public Result Subscribe(object o, uint msgType, Action<Msg> action)
        {
            using (var wlock = new WriteLock(rwLock))
            {
                if (subs.ContainsKey(msgType))
                {
                    var typeSubs = subs[msgType];

                    return typeSubs.Subscribe(
                        new Sub
                        {
                            Owner = o,
                            MsgType = msgType,
                            Action = action
                        });
                }
                else
                {
                    var typeSubs = new TypeSubs(msgType);
                    subs[msgType] = typeSubs;

                    return typeSubs.Subscribe(
                        new Sub
                        {
                            Owner = o,
                            MsgType = msgType,
                            Action = action
                        });
                }
            } 
        }

        public int Post(Msg m)
        {
            TypeSubs typeSubs = null;

            using (var rlock = new ReadLock(rwLock))
            {
                if (subs.ContainsKey(m.Type))
                {
                    typeSubs = subs[m.Type];
                }
            }

            if ( typeSubs != null )
            {
                return typeSubs.Post(m);
            }

            return 0;
        }

        public int GetSubscriptionCount(uint msgType)
        {
            using (var rlock = new ReadLock(rwLock))
            {
                if (subs.ContainsKey(msgType))
                {
                    var typeSubs = subs[msgType];
                    return typeSubs.GetSubscriptionCount();
                }
            }

            return 0;
        }

        public int GetSubscriptionCount(object o)
        {
            int count = 0;

            using (var wlock = new WriteLock(rwLock))
            {
                foreach (var kv in subs)
                {
                    count += kv.Value.GetSubscriptionCount(o);
                }
            }

            return count;
        }

        public void Unsubscribe(object o, uint msgType)
        {
            using (var wlock = new WriteLock(rwLock))
            {
                if (subs.ContainsKey(msgType))
                {
                    var typeSubs = subs[msgType];

                    typeSubs.Unsubscribe(o, msgType);
                }
            }
        }

        public void Unsubscribe(object o)
        {
            using (var wlock = new WriteLock(rwLock))
            {
                foreach ( var kv in subs )
                {
                    kv.Value.Unsubscribe(o);
                }
            }
        }
    }
}
