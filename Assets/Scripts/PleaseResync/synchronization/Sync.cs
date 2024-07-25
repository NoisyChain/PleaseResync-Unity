using System.Diagnostics;
using System.Collections.Generic;
using System;
//using UnityEngine;

namespace PleaseResync
{
    internal class Sync
    {
        public enum SyncState { SYNCING, RUNNING, DEVICE_LOST, DESYNCED }

        private readonly uint _inputSize;
        private readonly Device[] _devices;

        private TimeSync _timeSync;
        private InputQueue[] _deviceInputs;
        private StateStorage _stateStorage;

        private const int HealthCheckFramesBehind = 10;
        private const byte HealthCheckTime = 30;
        private byte HealthCheckBus;
        private bool _offlinePlay;

        private SyncState _syncState;

        public Sync(Device[] devices, uint inputSize, bool offline)
        {
            _devices = devices;
            _inputSize = inputSize;
            _offlinePlay = offline;
            _timeSync = new TimeSync();
            _stateStorage = new StateStorage(TimeSync.MaxRollbackFrames);
            _deviceInputs = new InputQueue[_devices.Length];
            _syncState = SyncState.SYNCING;
        }

        public void AddRemoteInput(uint deviceId, int frame, PlayerInput[] deviceInput)
        {
            // only allow adding input to the local device
            Debug.Assert(_devices[deviceId].Type == Device.DeviceType.Remote);
            // update device variables if needed
            if (_devices[deviceId].RemoteFrame < frame)
            {
                _devices[deviceId].RemoteFrame = frame;
                _devices[deviceId].RemoteFrameAdvantage = _timeSync.LocalFrame - frame;
                // let them know u recieved the packet
                _devices[deviceId].SendMessage(new DeviceInputAckMessage { Frame = (uint)frame });
            }
            AddDeviceInput(frame, deviceId, deviceInput);
        }

        public void SetLocalDevice(uint deviceId, uint playerCount, uint frameDelay)
        {
            _deviceInputs[deviceId] = new InputQueue(_inputSize, playerCount, frameDelay);
        }

        public void AddRemoteDevice(uint deviceId, uint playerCount)
        {
            _deviceInputs[deviceId] = new InputQueue(_inputSize, playerCount);
        }

        public void AddSpectatorDevice(uint deviceId)
        {
            _deviceInputs[deviceId] = new InputQueue(_inputSize, 1);
        }

        public List<SessionAction> AdvanceSync(uint localDeviceId, PlayerInput[] deviceInput)
        {
            // should be called after polling the remote devices for their messages.
            Debug.Assert(deviceInput != null);

            bool isTimeSynced = _offlinePlay ? true : _timeSync.IsTimeSynced(_devices);
            _syncState = isTimeSynced ? SyncState.RUNNING : SyncState.SYNCING;

            UpdateSyncFrame();

            var actions = new List<SessionAction>();

            if (!_offlinePlay)
            {                
                // create savestate at the initialFrame to support rolling back to it
                // for example if initframe = 0 then 0 will be first save option to rollback to.
                if (_timeSync.LocalFrame == TimeSync.InitialFrame)
                {
                    actions.Add(new SessionSaveGameAction(_timeSync.LocalFrame, _stateStorage));
                }

                // rollback update
                if (_timeSync.ShouldRollback())
                {
                    actions.Add(new SessionLoadGameAction(_timeSync.SyncFrame, _stateStorage));
                    for (int i = _timeSync.SyncFrame + 1; i <= _timeSync.LocalFrame; i++)
                    {
                        actions.Add(new SessionAdvanceFrameAction(i, GetFrameInput(i).Inputs));
                        actions.Add(new SessionSaveGameAction(i, _stateStorage));
                    }

                    UnityEngine.Debug.Log($"Rollback detected from frame {_timeSync.SyncFrame + 1} to frame {_timeSync.LocalFrame} ({RollbackFrames() + 1} frames)");
                }

                if (isTimeSynced)
                {
                    HealthCheck(); //<<< Working now I guess

                    _timeSync.LocalFrame++;

                    AddLocalInput(localDeviceId, deviceInput);
                    SendLocalInputs(localDeviceId);

                    actions.Add(new SessionAdvanceFrameAction(_timeSync.LocalFrame, GetFrameInput(_timeSync.LocalFrame).Inputs));
                    actions.Add(new SessionSaveGameAction(_timeSync.LocalFrame, _stateStorage));
                }
            }
            else
            {
                _timeSync.LocalFrame++;

                AddLocalInput(localDeviceId, deviceInput);

                actions.Add(new SessionAdvanceFrameAction(_timeSync.LocalFrame, GetFrameInput(_timeSync.LocalFrame).Inputs));
            }
            return actions;
        }

        private void HealthCheck()
        {
            SendHealthCheck();
            CheckHealth();
        }

        public void LookForDisconnectedDevices()
        {
            if (_syncState == SyncState.DESYNCED) return;
            
            foreach (var device in _devices)
            {
                if (device.State == Device.DeviceState.Disconnected)
                {
                    _syncState = SyncState.DEVICE_LOST;
                }
            }
        }

        private void SendLocalInputs(uint localDeviceId)
        {
            foreach (var device in _devices)
            {
                if (device.Type == Device.DeviceType.Remote)
                {
                    //Using a somewhat fixed value for the starting frame to compensate packet loss
                    //TODO: replace it for something more optimized
                    uint limitFrames = TimeSync.MaxRollbackFrames - 1;
                    uint startingFrame = _timeSync.LocalFrame <= limitFrames ? 0 : (uint)_timeSync.LocalFrame - limitFrames;
                    uint finalFrame = (uint)(_timeSync.LocalFrame + _deviceInputs[localDeviceId].GetFrameDelay());

                    var combinedInput = new List<PlayerInput>();

                    for (uint i = startingFrame; i <= finalFrame; i++)
                    {
                        combinedInput.AddRange(GetDeviceInput((int)i, localDeviceId).Inputs);
                    }

                    device.SendMessage(new DeviceInputMessage
                    {
                        StartFrame = startingFrame,
                        EndFrame = finalFrame,
                        Input = combinedInput.ToArray()
                    });
                }
            }
        }

        private void SendHealthCheck()
        {
            int frame = _timeSync.LocalFrame - HealthCheckFramesBehind;
            if (frame <= 0) return;

            uint checksum = _stateStorage.GetChecksum(frame);

            HealthCheckBus++;
            if (HealthCheckBus == HealthCheckTime)
            {
                foreach (var device in _devices)
                {
                    if (device.Type == Device.DeviceType.Remote)
                    {
                        device.SendMessage(new HealthCheckMessage
                        {
                            Frame = frame,
                            Checksum = checksum
                        });
                        UnityEngine.Debug.Log($"Sending HealthCheck message: {frame}, {checksum}");
                    }
                }
                HealthCheckBus = 0;
            }
        }

        private void CheckHealth()
        {
            foreach (var device in _devices)
            {
                if (device.Type == Device.DeviceType.Remote)
                {
                    foreach ((int, uint) health in device.Health)
                    {
                        if (!_stateStorage.CompareChecksums(health.Item1, health.Item2))
                        {
                            device.State = Device.DeviceState.Disconnected;
                            _syncState = SyncState.DESYNCED;
                            UnityEngine.Debug.Log($"State mismatch found.({health.Item2} : {_stateStorage.GetChecksum(health.Item1)})");
                            break;
                        }
                    }
                    device.Health.Clear();
                }
            }
        }

        private void UpdateSyncFrame()
        {
            if (_offlinePlay) return;
            int finalFrame = _timeSync.RemoteFrame;
            if (_timeSync.RemoteFrame > _timeSync.LocalFrame)
            {
                finalFrame = _timeSync.LocalFrame;
            }
            bool foundMistake = false;
            int foundFrame = finalFrame;
            for (int i = _timeSync.SyncFrame + 1; i <= finalFrame; i++)
            {
                foreach (var input in _deviceInputs)
                {
                    var predInput = input.GetPredictedInput(i);
                    if (predInput.Frame == i &&
                        input.GetInput(i, false).Frame == i)
                    {
                        // Incorrect Prediction
                        if (!predInput.Equal(input.GetInput(i, false), true))
                        {
                            foundFrame = i - 1;
                            foundMistake = true;
                        }
                        // remove prediction form queue
                        input.ResetPrediction(i);
                    }
                }
                if (foundMistake) break;
            }
            _timeSync.SyncFrame = foundFrame;
        }

        private void AddLocalInput(uint deviceId, PlayerInput[] deviceInput)
        {
            // only allow adding input to the local device
            Debug.Assert(_devices[deviceId].Type == Device.DeviceType.Local);
            AddDeviceInput(_timeSync.LocalFrame, deviceId, deviceInput);
        }
        
        private void AddDeviceInput(int frame, uint deviceId, PlayerInput[] deviceInput)
        {
            Debug.Assert(deviceInput.Length == _devices[deviceId].PlayerCount * _inputSize,
             "the length of the given deviceInput isnt correct!");

            var input = new GameInput(frame, _inputSize, _devices[deviceId].PlayerCount);
            input.SetInputs(0, _devices[deviceId].PlayerCount, deviceInput);

            _deviceInputs[deviceId].AddInput(frame, input);
        }

        private GameInput GetDeviceInput(int frame, uint deviceId)
        {
            return _deviceInputs[deviceId].GetInput(frame);
        }

        public GameInput GetFrameInput(int frame)
        {
            uint playerCount = 0;
            foreach (var device in _devices)
            {
                playerCount += device.PlayerCount;
            }
            // add all device inputs into a single GameInput
            var input = new GameInput(frame, _inputSize, playerCount);
            // offset is needed to put the players input in the correct position
            uint playerOffset = 0;
            for (uint i = 0; i < _devices.Length; i++)
            {
                // get the input of the device and add it to the rest of the inputs
                var tmpInput = GetDeviceInput(frame, i);
                input.SetInputs(playerOffset, _devices[i].PlayerCount, tmpInput.Inputs);
                // advance player offset to the position of the next device
                playerOffset += _devices[i].PlayerCount;
            }
            return input;
        }

        public int Frame() => _timeSync.LocalFrame;
        public int FramesAhead() => _timeSync.LocalFrameAdvantage;
        public uint RollbackFrames() => (uint) UnityEngine.Mathf.Max(0, _timeSync.LocalFrame - (_timeSync.SyncFrame + 1));
        public SyncState State() => _syncState;
    }
}