namespace PleaseResync
{
    internal class TimeSync
    {
        public const int InitialFrame = 0;
        public const int MaxRollbackFrames = 8;
        public const int FrameAdvantageLimit = 4;
        public int SyncFrame;
        public int LocalFrame;
        public int RemoteFrame;
        public int RemoteFrameAdvantage;
        public int LocalFrameAdvantage;

        public TimeSync()
        {
            SyncFrame = InitialFrame;
            LocalFrame = InitialFrame;
            RemoteFrame = InitialFrame;
            RemoteFrameAdvantage = InitialFrame;
            LocalFrameAdvantage = 0;
        }

        public bool IsTimeSynced(Device[] devices)
        {
            int minRemoteFrame = int.MaxValue;
            int maxRemoteFrameAdvantage = int.MinValue;

            foreach (var device in devices)
            {
                if (device.Type == Device.DeviceType.Remote)
                {
                    // find min remote frame
                    if (device.RemoteFrame < minRemoteFrame)
                    {
                        minRemoteFrame = device.RemoteFrame;
                    }
                    // find max frame advantage
                    if (device.RemoteFrameAdvantage > maxRemoteFrameAdvantage)
                    {
                        maxRemoteFrameAdvantage = device.RemoteFrameAdvantage;
                    }
                }
            }
            // Set variables
            RemoteFrame = minRemoteFrame;
            RemoteFrameAdvantage = maxRemoteFrameAdvantage;
            // How far the client is ahead of the last reported frame by the remote clients           
            LocalFrameAdvantage = LocalFrame - RemoteFrame;
            int frameAdvDiff = LocalFrameAdvantage - RemoteFrameAdvantage;
            // Only allow the local client to get so far ahead of remote.
            return LocalFrameAdvantage < MaxRollbackFrames && frameAdvDiff <= FrameAdvantageLimit;
        }

        public bool ShouldRollback()
        {
            // No need to rollback if we don't have a frame after the previous sync frame to synchronize to.
            return LocalFrame > SyncFrame && RemoteFrame > SyncFrame;
        }

        /*public bool PredictionLimitReached()
        {
            return LocalFrame >= MaxRollbackFrames && FrameAdvantage >= MaxRollbackFrames;
            //return !IsSynced();
        }

        public int GetMaxRollBackFrames()
        {
            return MaxRollbackFrames;
        }

        public bool IsSynced()
        {
            var local_frame_advantage = LocalFrame - RemoteFrame;                             //How far the client is ahead of the last reported frame by the remote client
            var frame_advantage_difference = local_frame_advantage - FrameAdvantage;          //How different is the frame advantage reported by the remote client and this one.
            return local_frame_advantage < MaxRollbackFrames && frame_advantage_difference <= FrameAdvantageLimit;
        }*/
    }
}
